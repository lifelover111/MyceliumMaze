using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//namespace OldProject;

/*
public abstract class Enemy : MonoBehaviour, IFacingMover, IHavingConcentration
{
    [Header("Set in Inspector: Enemy")]
    protected string name;
    public float maxHealth = 1;
    public float knockbackSpeed = 5;
    public float invincibleDuration = 0.1f;
    public float speed = 2;
    public float maxConcentration;
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

    private InRoom inRm;
    public int facing = 0;
    private float nextSpawnDirtTime;
    private float invincibleDone = 0;
    private Vector3 knockbackVel;
    protected Animator _anim;
    public Animator anim { get { return _anim; } }
    protected Rigidbody _rigid;
    public Rigidbody rigid { get { return _rigid; } protected set { _rigid = value; } }
    protected SpriteRenderer sRend;

    protected GameObject _target;
    public GameObject target { get { return _target; } }

    public Stats stats;

    protected List<GameObject> atkHitBoxes;
    public GameObject currentAtkHitbox;
    public Vector3 atkStepVel;

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

    protected AudioSource audioSource;
    protected AudioClip hitClip;
    protected AudioClip damageClip;
    protected AudioClip dieClip;
    protected AudioClip blockClip;
    protected AudioClip parryClip;
    protected AudioClip stepClip;
    float timeBetweenSteps = 0.3f;
    float stepDone;
    protected bool hit = false;

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        sRend = GetComponent<SpriteRenderer>();
        inRm = GetComponent<InRoom>();
        audioSource = GetComponent<AudioSource>();
        atkHitBoxes = new List<GameObject>();

        maxHealth = stats.Vitality * healthCoeff;
        maxConcentration = stats.Endurance * concCoeff;
        health = maxHealth;
        concentration = 0;

        behaviors = new List<IEnemyBehavior>();
        behaviorsDelegates = new List<BehaviorDelegate>();
    }


    private void Start()
    {
        facing = Random.Range(0, 8);
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


        if (Time.time - behaviorChoosedTime >= bhvrDecisiontime)
        {
            ChooseBehavior();
            behaviorChoosedTime = Time.time;
        }
        CurrentBehaviorDelegate.Invoke();

        viewDirection = new Vector3(Mathf.Cos(facing * Mathf.PI / 4), 0, Mathf.Sin(facing * Mathf.PI / 4));


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

            audioSource.clip = hitClip;
            audioSource.Play();

            hit = false;
        }

        avoidDecisionPosition = gameObject.transform.position;
    }


    private void FixedUpdate()
    {
        if (rigid.velocity.magnitude > 1.8f)
        {
            if (Time.time >= nextSpawnDirtTime)
            {
                DirtParticleSystemHandler.Instance?.SpawnDirt(transform.position, rigid.velocity * -0.2f);
                nextSpawnDirtTime = Time.time + 0.285f;
            }
            if (Time.time - stepDone > timeBetweenSteps)
            {
                stepDone = Time.time;
                audioSource.clip = stepClip;
                audioSource.Play();
            }
        }
        if (!isAttacking)
        {
            atkStepVel = Vector3.zero;
            foreach (GameObject go in atkHitBoxes)
                go.SetActive(false);
        }
    }


    private void ChooseTarget()
    {
        _target = HeroKeeper.instance.playerList[0].gameObject;
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
            if (CalcFacing(blockDirection) == facing)
            {
                if (Time.time - blockPlacedTime <= parryTimeWindow)
                {
                    coll.gameObject.GetComponentInParent<IHavingConcentration>().IncreaseConcentration(stats.Strength * 2);
                    agressivity *= 4;
                    audioSource.clip = parryClip;
                    audioSource.Play();
                    return; //parry
                }

                Vector3 delta = transform.position - coll.transform.position;

                knockbackVel = delta * knockbackSpeed * 0.5f;
                rigid.velocity = knockbackVel;

                audioSource.clip = blockClip;
                audioSource.Play();

                concentration += dEf.concentrationDamage;
                ConcentrationOverflowCheck();
                agressivity *= 2f;
                blockPlacedTime = Time.time; //сброс времени блока для предотвращения абуза регенерации концентрации в блоке

                return;
            }
        }
        health -= dEf.damage * (1 + concentration / maxConcentration);

        knockback = true;
        anim.CrossFade(name + "_Damage_" + facing, 0);
        anim.speed = 2f;

        audioSource.clip = damageClip;
        audioSource.Play();

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
            knockback = true;
        }

    }


    void ConcentrationOverflowCheck()
    {
        if (concentration >= maxConcentration)
        {
            knockback = true;
            rigid.velocity = Vector2.zero;
            knockbackVel = Vector2.zero;
            anim.CrossFade(name + "_Stun_" + facing, 0);
            anim.speed = 1.5f;
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
        sRend.sortingLayerName = "Default";
        InvokeRepeating("FadeSprite", 2, 0.1f);
        knockback = true;
        OldProject.Door.removeInRoom(inRm);
        anim.CrossFade(name + "_Die_" + facing, 0);
        anim.speed = 2;
        audioSource.clip = dieClip;
        audioSource.Play();
        Invoke("Die", 2.5f);
    }

    void FadeSprite()
    {
        sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, Mathf.Lerp(sRend.color.a, 0, 0.1f));
    }

    void Die()
    {
        foreach (var hero in HeroKeeper.instance.playerList)
        {
            //hero.souls += soulsAfterDeath / HeroKeeper.instance.playerList.Count;
        }

        Destroy(gameObject);
    }

    public int GetFacing()
    {
        return facing;
    }

    public bool moving { get { return true; } }

    public float GetSpeed()
    {
        return speed;
    }
    public float gridMult
    {
        get { return inRm.gridMult; }
    }
    public Vector2 roomPos
    {
        get { return inRm.roomPos; }
        set { inRm.roomPos = value; }
    }
    public Vector2 roomNum
    {
        get { return inRm.roomNum; }
        set { inRm.roomNum = value; }
    }
    public Vector2 GetRoomPosOnGrid(float mult = -1)
    {
        return inRm.GetRoomPosOnGrid(mult);
    }

    public int CalcFacing(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.z, direction.x) * (180 / Mathf.PI);
        if (angle < 0) angle += 360;
        int facing = (int)Mathf.Floor((angle + 22.5f) / 45);
        if (facing == 8) facing = 0;

        facing = facing == 0 ? 7 : facing - 1;

        return facing;
    }

    int CalcSoulsAfterDeath()
    {
        return soulsPerLevel * stats.Level;
    }

    public void SetStats(Stats stats)
    {
        this.stats = stats;
        maxHealth = stats.Vitality * healthCoeff;
        maxConcentration = stats.Endurance * concCoeff;
        health = maxHealth;
        concentration = 0;

        foreach (IEnemyBehavior bhvr in behaviors)
            bhvr.InitNewStats();
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
        atkStepVel = 1.3f * speed * viewDirection;
        currentAtkHitbox.transform.localRotation = Quaternion.Euler(45, 0, 45 * facing);
        currentAtkHitbox.SetActive(true);

        if (Random.value > 0.5)
            agressivity *= 0.5f;

        rigid.velocity = atkStepVel;
    }
    void StopHit()
    {
        canChooseBehavior = true;
        atkStepVel = Vector2.zero;
        currentAtkHitbox.SetActive(false);
        isAttacking = false;

        rigid.velocity = atkStepVel;
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
*/