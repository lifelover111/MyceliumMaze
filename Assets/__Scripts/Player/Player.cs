using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour, IHavingConcentration
{
    public enum eMode { idle, move, attack, transition, knockback, block, rest, die, healing, dash }

    public float speedForward;
    public float speedBackward;
    public float speedToSide;

    
    public float damage = 2000;
    public float concentrationDamage = 5;
    public float parryTimeWindow = 0.2f;
    private float blockPlacedTime;

    float concentartionRestoreDelay = 4f;
    float timeConcentrationRestorationStopped;

    public float maxConcentration = 20;
    private float _concentration = 0;
    public float concentration { get { return _concentration; } set { _concentration = value > maxConcentration ? maxConcentration : value; } }
    public int maxHealth = 10;
    public int maxSanity = 20;
    private float _health;
    private float _sanity;
    public float health { get { return _health; } set { _health = value > maxHealth ? maxHealth : value; } }
    public float sanity { get { return _sanity; } set { _sanity = value; } }

    public int spores = 0;
    public int maxNumFlasks = 3;
    public int numFlasks;

    public event Action OnSporeCountChanged = delegate { };
    public event Action OnFlaskCountChanged = delegate { };

    bool invincible = false;
    float invincibleDone;
    float invincibleDuration = 0.5f;

    public bool _IsControlledByAnimator = false;
    public Collider weapon;
    float speed;
    [SerializeField] Animator anim;
    bool dirHeld = false;
    Vector3 dir;
    public eMode mode = eMode.idle;

    private Rigidbody rigid;
    Vector3 viewDirection;
    public event System.Action OnControlledByAnimator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        var dmgEffect = weapon.GetComponent<DamageEffect>();
        dmgEffect.damage = damage;
        dmgEffect.concentrationDamage = concentrationDamage;
    }

    private void Start()
    {
        health = maxHealth;
        sanity = maxSanity;
        numFlasks = maxNumFlasks;
        spores = 0;
        OnSporeCountChanged();
        OnFlaskCountChanged();
    }

    void Update()
    {
        if (invincible && Time.time > invincibleDone)
            invincible = false;

        if(mode == eMode.knockback)
        {
            return;
        }

        viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection = new Vector3(viewDirection.x, 0, viewDirection.z).normalized;

        if (mode != eMode.idle)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.left, viewDirection), 2.2f);
        }

        dirHeld = false;
        if (anim.GetBool("Idle"))
        {
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            dir = Quaternion.Euler(0, -45, 0) * dir;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                dir = transform.rotation * Vector3.left;
            }

            if (dir.magnitude > 0)
            {
                dirHeld = true;
            };
            dir.Normalize();

            if (!dirHeld)
            {
                mode = eMode.idle;
            }
            else
                mode = eMode.move;
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            transform.rotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), viewDirection);
            anim.SetTrigger("Attack");
            mode = eMode.attack;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            anim.SetBool("Block", true);
            blockPlacedTime = Time.time;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            anim.SetBool("Block", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            mode = eMode.dash;
        }

        if(anim.GetBool("Block"))
        {
            mode = eMode.block;
        }

        if (_IsControlledByAnimator)
        {
            OnControlledByAnimator?.Invoke();
            return; 
        }

        RestoreConcentration();

        Vector3 vel = Vector3.zero;
        switch (mode)
        {
            case eMode.idle:
                anim.SetFloat("x", 0);
                anim.SetFloat("y", 0);

                float p = Mathf.Sin(Mathf.Deg2Rad * Vector3.SignedAngle(transform.rotation * Vector3.left, viewDirection, Vector3.up));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.left, viewDirection), 2.2f);

                anim.SetFloat("turn", p);
                break;
            case eMode.move:
                vel = dir;
                SetSpeed(dir);
                float angle = Vector2.SignedAngle(new Vector2(viewDirection.x, viewDirection.z), Vector2.up);
                Vector3 walkTree = (Quaternion.AngleAxis(angle, Vector3.down) * dir).normalized;
                anim.SetFloat("x", walkTree.x);
                anim.SetFloat("y", walkTree.z);
                break;
            case eMode.dash:
                anim.SetTrigger("Dash");
                float aangle = Vector2.SignedAngle(new Vector2(viewDirection.x, viewDirection.z), Vector2.up);
                Vector3 dashTree = (Quaternion.AngleAxis(aangle, Vector3.down) * dir).normalized;
                anim.SetFloat("x", Mathf.RoundToInt(dashTree.x));
                anim.SetFloat("y", Mathf.RoundToInt(dashTree.z));
                break;
        }
        rigid.velocity = vel * speed;
    }


    private void OnTriggerEnter(Collider coll)
    {
        if (invincible)
            return;
        DamageEffect dEf = coll.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return;

        if (mode == eMode.block)
        {
            Vector3 blockDirection = coll.gameObject.transform.root.position - transform.position;
            if (Vector3.Angle(viewDirection, blockDirection) < 45f)
            {
                if (Time.time - blockPlacedTime <= parryTimeWindow)
                {
                    if (coll.gameObject.tag != "Projectile")
                        coll.gameObject.GetComponentInParent<IHavingConcentration>().IncreaseConcentration(concentrationDamage * 2);

                    anim.SetBool("Block", false);
                    anim.SetTrigger("Parry");
                    return; //parry
                }

                concentration += dEf.concentrationDamage;
                ConcentrationOverflowCheck();
                blockPlacedTime = Time.time; //сброс времени блока для предотвращения абуза регенерации концентрации в блоке
                anim.SetTrigger("BlockHit");
                return;
            }
        }
        health -= dEf.damage;
        anim.SetBool("Block", false);
        anim.SetTrigger("Hit");

        concentration += dEf.concentrationDamage;
        ConcentrationOverflowCheck();
        
        invincible = true;
        invincibleDone = Time.time + invincibleDuration;

        /*
        StopAttack();
        if (dEf.knockback)
        {
            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;
            mode = eMode.knockback;
            knockbackDone = Time.time + knockbackDuration;
        }

        if (health <= 0)
            Die();
        */
    }

    void RestoreConcentration()
    {
        if (mode == eMode.attack || mode == eMode.die || mode == eMode.knockback || mode == eMode.dash)
            return;
        if (mode == eMode.block && Time.time > timeConcentrationRestorationStopped + concentartionRestoreDelay/2)
        {
            if (concentration > 0)
            {
                concentration -= 1.5f*Time.deltaTime;
            }
            return;
        }
        if (Time.time > timeConcentrationRestorationStopped + concentartionRestoreDelay)
        {
            if (concentration > 0)
            {
                concentration -= 1.5f * Time.deltaTime;
            }
        }
    }

    public void GiveSpores(int sporeCount)
    {
        spores += sporeCount;
        OnSporeCountChanged?.Invoke();
    }

    public bool TryTakeSpores(int sporeCount)
    {
        if (sporeCount > spores)
        {
            return false;
        }

        spores -= sporeCount;
        OnSporeCountChanged?.Invoke();
        return true;
    }

    void SetSpeed(Vector3 direction)
    {
        float angle = Vector3.SignedAngle(viewDirection, direction, Vector3.down);
        if (angle < 0) angle += 360;

        if ((angle >= 0 && angle < 30) || angle > 330)
        {
            speed = speedForward;
        }
        else if (angle > 150 && angle < 210)
        {
            speed = speedBackward;
        }
        else
        {
            speed = speedToSide;
        }
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
    void ConcentrationOverflowCheck()
    {
        timeConcentrationRestorationStopped = Time.time;
        if (concentration >= maxConcentration)
        {
            anim.SetTrigger("Stun");
            mode = eMode.knockback;
            concentration = 0;
        }
    }
}
