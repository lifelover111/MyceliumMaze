using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHavingConcentration
{
    [Header("Set in Inspector: Enemy")]
    protected string name;
    public float maxHealth = 20;
    public float knockbackSpeed = 5;
    public float invincibleDuration = 0.5f;
    public float speed = 2;
    public float maxConcentration = 30;
    public float parryTimeWindow = 0.2f;
    public GameObject concentrationBarPrefab;
    public GameObject healthBarPrefab;

    public float baseAgressivity = 20;

    protected float healthCoeff;
    protected float concCoeff;


    [Header("Set Dynamically: Enemy")]
    public float health;
    private float _concentration;
    public float concentration { get { return _concentration; } set { _concentration = value > maxConcentration ? maxConcentration : value; } }
    public bool invincible = false;
    public bool knockback = false;
    public float agressivity;

    private int soulsAfterDeath;
    protected int soulsPerLevel;

    private float invincibleDone = 0;
    private Vector3 knockbackVel;
    protected Animator _anim;
    public Animator anim { get { return _anim; } }
    protected Rigidbody _rigid;
    public Rigidbody rigid { get { return _rigid; } protected set { _rigid = value; } }

    protected GameObject _target;
    public GameObject target { get { return _target; } }


    public GameObject weapon;
    

    public bool isBlockUp = false;
    public float blockPlacedTime;

    float bhvrDecisiontime = 0.5f;
    float behaviorChoosedTime = 0;

    protected List<IEnemyBehavior> behaviors;
    private delegate void BehaviorDelegate();
    private List<BehaviorDelegate> behaviorsDelegates;
    private BehaviorDelegate CurrentBehaviorDelegate;
    public bool canChooseBehavior = true;
    public bool isAttacking = false;

    public float lastMoveTime = 0;
    public Vector3 avoidDecisionPosition;
    private Vector3 viewDirection;

    float timeBetweenSteps = 0.3f;
    float stepDone;
    protected bool hit = false;

    protected virtual void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();

        health = maxHealth;
        concentration = 0;

        behaviors = new List<IEnemyBehavior>();
        behaviorsDelegates = new List<BehaviorDelegate>();
    }


    private void Start()
    {
        GameObject go = Instantiate(concentrationBarPrefab);
        go.transform.SetParent(transform, false);
        go = Instantiate(healthBarPrefab);
        go.transform.SetParent(transform, false);


        ChooseTarget();
        foreach (IEnemyBehavior bhvr in behaviors)
            behaviorsDelegates.Add(bhvr.Update);
        agressivity = baseAgressivity;
        behaviorChoosedTime = -bhvrDecisiontime;

        soulsAfterDeath = CalcSoulsAfterDeath();

        lastMoveTime = Time.time;
    }


    protected virtual void Update()
    {
        if (invincible && Time.time > invincibleDone)
            invincible = false;

        if (knockback)
        {
            rigid.velocity = knockbackVel;
            return;
        }
        knockback = false;

        if (target != null)
            viewDirection = (target.transform.position - transform.position).normalized;
        if (anim.GetBool("Idle"))
        {
            anim.SetFloat("x", 0);
            anim.SetFloat("y", 0);

            float p = Mathf.Sin(Mathf.Deg2Rad * Vector3.SignedAngle(transform.rotation * Vector3.left, viewDirection, Vector3.up));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.left, viewDirection), 2.2f);

            anim.SetFloat("turn", p);
        }
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.left, viewDirection), 2.2f);


        if (Time.time - behaviorChoosedTime >= bhvrDecisiontime)
        {
            ChooseBehavior();
            behaviorChoosedTime = Time.time;
        }
        CurrentBehaviorDelegate.Invoke();

        

        


        if (concentration > 0)
        {
            if (isBlockUp)
            {
                int concDecreaseCoeff = (Time.time - blockPlacedTime > 2) ? 4 : 1;
                concentration -= concDecreaseCoeff * Time.deltaTime;
            }
            else
                concentration -= Time.deltaTime;
        }

        if (hit)
        {
            
            //audioSource.clip = hitClip;
            //audioSource.Play();

            hit = false;
        }

        avoidDecisionPosition = gameObject.transform.position;
    }


    private void FixedUpdate()
    {
        if (rigid.velocity.magnitude > 1.8f)
        {
            /*
            if (Time.time >= nextSpawnDirtTime)
            {
                DirtParticleSystemHandler.Instance?.SpawnDirt(transform.position, rigid.velocity * -0.2f);
                nextSpawnDirtTime = Time.time + 0.285f;
            }*/
            if (Time.time - stepDone > timeBetweenSteps)
            {
                stepDone = Time.time;
                //audioSource.clip = stepClip;
                //audioSource.Play();
            }
        }
    }


    private void ChooseTarget()
    {
        _target = PlayerManager.instance.playerList[0].gameObject;
    }

    void ChooseBehavior()
    {
        if (!canChooseBehavior)
            return;

        float[] analyzeResults = new float[behaviors.Count];
        for (int i = 0; i < behaviors.Count; i++)
        {
            analyzeResults[i] = behaviors[i].Analyze();
        }
        float maxAnalyze = System.Linq.Enumerable.Max(analyzeResults);
        List<int> bhvrPriorityIndexes = new List<int>();
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (analyzeResults[i] == maxAnalyze)
                bhvrPriorityIndexes.Add(i);
        }
        int index = bhvrPriorityIndexes[Random.Range(0, bhvrPriorityIndexes.Count)];
        CurrentBehaviorDelegate = behaviorsDelegates[index];
        behaviors[index].PrepareBehavior();
    }



    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag != "PlayerDamage")
            return;

        if (invincible)
            return;

        DamageEffect dEf = coll.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return;


        if (isBlockUp)
        {
            Vector3 blockDirection = coll.gameObject.transform.position - transform.position;
            if (Vector3.Angle(viewDirection, blockDirection) < 30f)
            {
                if (Time.time - blockPlacedTime <= parryTimeWindow)
                {
                    coll.gameObject.GetComponentInParent<IHavingConcentration>().IncreaseConcentration(weapon.GetComponent<DamageEffect>().concentrationDamage * 2);
                    agressivity *= 4;
                    //audioSource.clip = parryClip;
                    //audioSource.Play();
                    return; //parry
                }

                Vector3 delta = transform.position - coll.transform.position;

                knockbackVel = delta * knockbackSpeed * 0.5f;
                rigid.velocity = knockbackVel;

                //audioSource.clip = blockClip;
                //audioSource.Play();

                concentration += dEf.concentrationDamage;
                ConcentrationOverflowCheck();
                agressivity *= 2f;
                blockPlacedTime = Time.time; //сброс времени блока для предотвращения абуза регенерации концентрации в блоке

                return;
            }
        }
        health -= dEf.damage * (1 + concentration / maxConcentration);

        //knockback = true;
        anim.SetTrigger("Hit");

        //audioSource.clip = damageClip;
        //audioSource.Play();

        rigid.velocity = Vector3.zero;
        knockbackVel = Vector3.zero;
        concentration += dEf.concentrationDamage / 2;
        ConcentrationOverflowCheck();
        agressivity *= 0.25f;
        if (health <= 0)
        {
            DiePrepare();
            return;
        }


        invincible = true;
        invincibleDone = Time.time + invincibleDuration;
        if (dEf.knockback)
        {
            Vector3 delta = transform.position - coll.transform.position;

            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;
            //knockback = true;
        }

    }


    void ConcentrationOverflowCheck()
    {
        if (concentration >= maxConcentration)
        {
            knockback = true;
            rigid.velocity = Vector2.zero;
            knockbackVel = Vector2.zero;
            anim.SetTrigger("Hit");
            agressivity *= 0.001f;
            isBlockUp = false;
            isAttacking = false;
        }
    }

    void DiePrepare()
    {
        rigid.velocity = Vector2.zero;
        gameObject.GetComponent<Collider>().enabled = false;
        GetComponentInChildren<GuiConcentrationBar>().gameObject.SetActive(false);
        GetComponentInChildren<GuiHealthBar>().gameObject.SetActive(false);
        knockback = true;
        anim.speed = 2;
        //audioSource.clip = dieClip;
        //audioSource.Play();
        Invoke("Die", 2.5f);
    }


    void Die()
    {
        foreach (var player in PlayerManager.instance.playerList)
        {
            //hero.souls += soulsAfterDeath / HeroKeeper.instance.playerList.Count;
        }

        Destroy(gameObject);
    }

    public bool moving { get { return true; } }

    public float GetSpeed()
    {
        return speed;
    }

    int CalcSoulsAfterDeath()
    {
        //return soulsPerLevel * stats.Level;
        return 100;
    }

    public float GetConcentration()
    {
        return concentration;
    }

    public float GetMaxConcentration()
    {
        return maxConcentration;
    }
    public void IncreaseConcentration(float val)
    {
        concentration += val;
        ConcentrationOverflowCheck();
    }


    void StartHit()
    {
        hit = true;

        if (Random.value > 0.5)
            agressivity *= 0.5f;

    }
    void StopHit()
    {
        canChooseBehavior = true;
        isAttacking = false;
    }

    void RecoverAfterDamage()
    {
        concentration = 0;
        knockback = false;
        canChooseBehavior = true;
    }
    void PauseAnimation()
    {
        anim.speed = 0;
    }
}