using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour, IFacingMover, IHavingConcentration
{
    public enum eMode { idle, move, attack, transition, knockback, block, rest, die, healing }

    [Header("Set in Inspector")]
    public float speed = 5;
    public float transitionDelay = 2f;
    public int maxHealth = 10;
    public int maxSanity = 20;
    public float knockbackSpeed = 2;
    public float knockbackDuration = 0.1f;
    public float invincibleDuration = 0.5f;

    public float maxConcentration = 20;
    private float _concentration = 0;
    public float concentration { get { return _concentration; } set { _concentration = value > maxConcentration ? maxConcentration : value; } }
    public float damage = 5;
    public float concentrationDamage = 5;
    public float parryTimeWindow = 0.1f;

    [Header("Set Dynamically")]
    public bool dirHeld = false;
    public Vector3 dir;
    public int facing = 1;
    public eMode mode = eMode.idle;
    public int maxNumFlasks = 3;
    public int numFlasks = 3;
    public bool invincible = false;

    private float _health;
    private float _sanity;
    public float health { get { return _health; } set { _health = value > maxHealth ? maxHealth : value; } }
    public float sanity { get { return _sanity; } set { _sanity = value; } }

    public int souls = 0;

    private float transitionDone = 0;
    private float knockbackDone = 0;
    private float invincibleDone = 0;
    private Vector3 knockbackVel;
    private Rigidbody rigid;
    private Animator anim;
    private InRoom inRm;
    private DamageEffect damageEffect;
    private float nextSpawnDirtTime;

    private GameObject atkHitBox;
    private int atkCounter = 0;
    private Vector3 atkStepVel = Vector3.zero;
    private bool nextAtk = false;
    private bool animCancelable = true;

    public bool canRotate { get { return animCancelable; } }
    private bool blockInSchedule = false;
    private float blockPlacedTime;
    private float blockButtonUp;
    public int flaskCount {get { return numFlasks; } set { numFlasks = value; } }
    bool isHealing = false;
    public InRoom GetInRoom { get { return inRm; } }
    public Vector3 GetPosition() { return gameObject.transform.position; }

    System.Action updateStatsDelegate;

    private HeroStats stats;


    private float continousDamageTimeCounter;


    public float attackRange = 1.5f;

    private ViewDirectionController viewDirectionController;


    AudioSource audioSource;
    float timeBetweenSteps = 0.3f;
    float stepDone;
    int stepCounter = 0;

    private void Start()
    {
        Door.OnTransition += () =>
        {
            transitionDone = Time.time + transitionDelay;
            mode = eMode.transition;
        };
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        inRm = GetComponent<InRoom>();
        atkHitBox = transform.GetChild(0).gameObject;
        damageEffect = atkHitBox.GetComponent<DamageEffect>();
        InitStatsIfNeeded();
        health = maxHealth;
        sanity = maxSanity;
        concentration = 0;
        damageEffect.damage = damage;
        damageEffect.concentrationDamage = concentrationDamage;
        numFlasks = maxNumFlasks;
        viewDirectionController = GetComponent<ViewDirectionController>();
    }


    void Update()
    {
        if (mode == eMode.die)
        {
            rigid.velocity = Vector3.zero;
            return;
        }
        if (stats.isChanged)
        {
            updateStatsDelegate();
            stats.isChanged = false;
        }
        Vector3 viewDirection = viewDirectionController.viewDirection;
        if(mode != eMode.attack && mode != eMode.healing && mode != eMode.knockback && mode != eMode.rest)
            facing = CalcFacing(viewDirection);
        
        if (invincible && Time.time > invincibleDone) 
            invincible = false;
        
        if ( mode == eMode.knockback ) {
            rigid.velocity = knockbackVel*0.1f;
                return;
        }
        if ( mode == eMode.transition ) {
            DoorTransition();
            return;
        }
        if ( mode == eMode.rest )
            return;


        dirHeld = false;
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        dir = Quaternion.Euler(0, -45, 0) * dir;
        if ( dir.magnitude > 0 ) dirHeld = true;
        dir.Normalize();
       
        if (blockInSchedule && mode != eMode.attack && animCancelable)
        {
            mode = eMode.block;
            blockPlacedTime = Time.time;
            blockInSchedule = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (mode != eMode.healing)
            {
                if (mode != eMode.attack)
                {
                    animCancelable = true;
                    atkCounter = 0;
                    mode = eMode.attack;
                }
                else
                    nextAtk = true;
            }
        }



        if (Input.GetMouseButtonDown(1))
        {
            if (animCancelable)
            {
                mode = eMode.block;
                blockPlacedTime = Time.time;
            }
            else
            {
                blockInSchedule = true;
            }
        }

        else if(Input.GetMouseButtonUp(1))
        {
            blockInSchedule = false;
            blockButtonUp = Time.time;
            
        }
        if (mode == eMode.block && Time.time - blockButtonUp > 0.15f && !Input.GetKey(KeyCode.Mouse1))
        {
            mode = eMode.idle;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (mode != eMode.attack)
            {
                mode = eMode.healing;
                animCancelable = false;
                isHealing = false;
            }
        }



        if (mode != eMode.attack && mode != eMode.block && mode != eMode.healing) {
            if (!dirHeld) {
                mode = eMode.idle;
            } 
            else {
                mode = eMode.move;
            }
        }

        
       

        Vector3 vel = Vector3.zero;
        switch (mode) {
            case eMode.attack:
                anim.CrossFade( "Hero_Attack_"+facing+'_'+atkCounter%2, 0 );
                anim.speed = 1;
                vel = atkStepVel;
                if (vel.magnitude > 0.3f)
                {
                    if (Time.time >= nextSpawnDirtTime)
                    {
                        DirtParticleSystemHandler.Instance?.SpawnDirt(transform.position, vel * -0.4f);
                        nextSpawnDirtTime = Time.time + 0.5f;
                    }
                }
                break;
            case eMode.idle:
                anim.CrossFade( "Hero_Idle_"+facing, 0 );
                anim.speed = 1;
                if (concentration > 0)
                    concentration -= Time.deltaTime * sanity / maxSanity;
                break;
            case eMode.move:
                vel = dir;
                char c = CalcBodyDirection(dir);
                anim.CrossFade( "Hero_Move_"+c+facing, 0 );
                anim.speed = 1;
                if (Time.time >= nextSpawnDirtTime)
                {
                    DirtParticleSystemHandler.Instance?.SpawnDirt(transform.position, dir * -0.2f);
                    nextSpawnDirtTime = Time.time + 0.285f;   
                }
                if (concentration > 0)
                    concentration -= Time.deltaTime * sanity / maxSanity;

                if(Time.time - stepDone >= timeBetweenSteps)
                {
                    stepDone = Time.time;
                    audioSource.clip = stepCounter % 2 == 0 ? SoundBank.instance?.Footstep_Hero_0 : SoundBank.instance?.Footstep_Hero_1;
                    audioSource.Play();
                    stepCounter++;
                }

                break;
            case eMode.block:
                anim.CrossFade("Hero_Block_" + facing, 0);
                anim.speed = 1;
                if (concentration > 0)
                {
                    float concDecreaseCoeff = (Time.time - blockPlacedTime > 1) ? 4 * sanity / maxSanity : 1 * sanity / maxSanity;
                    concentration -= concDecreaseCoeff * Time.deltaTime;
                }
                if (Time.time < knockbackDone)
                {
                    vel = knockbackVel;
                    if (Time.time >= nextSpawnDirtTime)
                    {
                        DirtParticleSystemHandler.Instance?.SpawnDirt(transform.position, vel * -0.1f);
                        nextSpawnDirtTime = Time.time + 0.05f;
                    }
                }
                break;
            case eMode.healing:
                char res = flaskCount > 0 ? 'S' : 'F';
                if (!isHealing)
                {
                    anim.CrossFade("Hero_Heal_" + facing + '_' + res, 0);
                    anim.speed = 1;
                    isHealing = true;
                }
                break;
        }
        rigid.velocity = vel * speed;
    }


    void InitStatsIfNeeded()
    {
        if (stats != null)
        {
            return;
        }
        stats = new HeroStats(5, 5, 5, 5, 5);
        maxHealth = stats.Vitality * 4;
        damage = stats.Dexterity * 2;
        maxConcentration = stats.Endurance * 3;
        concentrationDamage = stats.Strength;
        maxSanity = stats.Will * 4;
        stats.isChanged = true;
        updateStatsDelegate = () => {
            float healthPercentage = health / maxHealth;
            float sanityPercentage = sanity / maxSanity;
            maxHealth = stats.Vitality * 4;
            maxSanity = stats.Will * 4;
            health = maxHealth * healthPercentage;
            sanity = maxSanity * sanityPercentage;
            damage = stats.Dexterity * 2;
            maxConcentration = stats.Endurance * 3;
            concentrationDamage = stats.Strength;
            damageEffect.damage = damage;
            damageEffect.concentrationDamage = concentrationDamage;

            GuiPanel.UpdateInfo();
        };
    }

    int CalcFacing(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.z, direction.x)*(180/Mathf.PI);
        if (angle < 0) angle += 360;
        int facing = (int)Mathf.Floor((angle + 22.5f)/45);
        if (facing == 8) facing = 0;

        facing = facing == 0 ? 7 : facing - 1;

        return facing;
    }

    char CalcBodyDirection(Vector3 direction)
    {
        char[] cd = { 'T', 'L', 'B', 'R' };
        Vector3 relDir = new Vector3(Mathf.Cos(facing * Mathf.PI / 4), 0, Mathf.Sin(facing * Mathf.PI / 4));
        float angle = Vector3.SignedAngle(relDir, direction, Vector3.down);
        if (angle < 0) angle += 360;
        int i = (int)Mathf.Floor((angle) / 90);
        if (i == 4) i = 0;
        return cd[i];
    }





    public int GetFacing() {
        return facing;
    }

    public bool moving {
        get { return (mode == eMode.move); }
    }

    public float GetSpeed() {
        return speed;
    }
    
    public float gridMult {
        get { return inRm.gridMult; }
    }

    public Vector2 roomPos {
        get { return inRm.roomPos; }
        set { inRm.roomPos = value; }
    }

    public Vector2 roomNum {
        get { return inRm.roomNum; }
        set { inRm.roomNum = value; }
    }

    public Vector2 GetRoomPosOnGrid( float mult = -1 ) {
        return inRm.GetRoomPosOnGrid( mult );
    }


    void LateUpdate()
    {
        if (mode != eMode.attack)
            atkHitBox.SetActive(false);
        /*
        Vector2 rPos = GetRoomPosOnGrid( 0.5f );
        int doorNum;
        for (doorNum=0; doorNum<4; doorNum++) 
        {
            if (rPos == InRoom.DOORS[doorNum]) 
            {
                break;
            }
        }
        if ( doorNum > 3 ) return;
        Vector2 rm = roomNum;
        switch (doorNum) {
            case 0:
                rm.x += 1;
                break;
            case 1:
                rm.y += 1;
                break;
            case 2:
                rm.x -= 1;
                break;
            case 3:
                rm.y -= 1;
                break;
        }
        if (rm.x >= 0 && rm.x <= InRoom.MAX_RM_X) {
            if (rm.y >=0 && rm.y <= InRoom.MAX_RM_Y) {
                roomNum = rm;
                transitionPos = InRoom.ENTERS[ (doorNum+2) % 4 ];
                roomPos = transitionPos;
                mode = eMode.transition;
                transitionDone = Time.time + transitionDelay;
            }
        }
        */
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (Time.time - continousDamageTimeCounter < 1)
            return;

        if (other.gameObject.tag != "ContinousDamage")
            return;
        
        continousDamageTimeCounter = Time.time;
        DamageEffect dmgEf = other.GetComponent<DamageEffect>();

        sanity -= dmgEf.sanityDamage;
        if(sanity <= 0)
        {
            Die();
        }

    }
    

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "ContinousDamage")
            return;
        if (mode == eMode.die)
            return;
        if (invincible) 
            return;
        DamageEffect dEf = coll.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return;

        Vector3 delta = transform.position - coll.transform.position;
        if (mode == eMode.block)
        {
            Vector3 blockDirection = coll.gameObject.transform.position - transform.position;
            if(Mathf.Abs(CalcFacing(blockDirection) - facing) <= 1 || Mathf.Abs(CalcFacing(blockDirection) - facing) >= 7)
            {
                if (Time.time - blockPlacedTime <= parryTimeWindow)
                {
                    if (coll.gameObject.tag != "Projectile")
                        coll.gameObject.GetComponentInParent<IHavingConcentration>().IncreaseConcentration(concentrationDamage * 2);
                    audioSource.clip = SoundBank.instance?.Parry;
                    audioSource.Play();
                    return; //parry
                }

                audioSource.clip = SoundBank.instance?.Block;
                audioSource.Play();

                if (dEf.knockback)
                {
                    knockbackVel = delta * knockbackSpeed * 0.5f;
                    rigid.velocity = knockbackVel;

                    knockbackDone = Time.time + knockbackDuration * 0.25f;
                }
                
                concentration += dEf.concentrationDamage;
                ConcentrationOverflowCheck();
                blockPlacedTime = Time.time; //сброс времени блока для предотвращения абуза регенерации концентрации в блоке
                return;
            }
        }
        health -= dEf.damage;
        audioSource.clip = SoundBank.instance?.Damage_Hero;
        audioSource.Play();
        anim.CrossFade("Hero_Damage_" + facing, 0);
        anim.speed = 2;

        concentration += dEf.concentrationDamage / 2;
        ConcentrationOverflowCheck();
        invincible = true;
        invincibleDone = Time.time + invincibleDuration;


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
    }


    void ConcentrationOverflowCheck()
    {
        if (concentration >= maxConcentration)
        {
            anim.CrossFade("Hero_Stun_" + facing, 0);
            anim.speed = mode == eMode.block ? 1 : 2;
            mode = eMode.knockback;
        }
    }


    void Die()
    {
        anim.CrossFade("Hero_Die_" + facing, 0);
        anim.speed = 1;
        mode = eMode.die;
        rigid.velocity = Vector3.zero;
        audioSource.clip = SoundBank.instance?.Death;
        System.Action playSound = delegate () { audioSource.Play(); };
        Invoke(playSound.Method.Name, 6.5f);
        DeathCamera.instance.Invoke("Load", 5f);
    }

    void DoorTransition()
    {
        /*
        rigid.velocity = Vector3.zero;
        anim.speed = 0;
        roomPos = transitionPos;
        */
        facing = CalcFacing(Quaternion.Euler(0,0,-45)*dir);
        anim.CrossFade("Hero_Move_T" + facing, 0);
        if (Time.time < transitionDone)
            return;
        mode = eMode.idle;
    }

    void MakeAtkStep()
    {
        if (mode == eMode.attack)
        {
            audioSource.clip = atkCounter%2 == 0? SoundBank.instance?.Footstep_Hero_0: SoundBank.instance?.Footstep_Hero_1;
            if (!audioSource.isPlaying)
                audioSource.Play();
            atkStepVel = 0.35f * new Vector3(Mathf.Cos((facing + 1) * Mathf.PI / 4), 0, Mathf.Sin((facing + 1) * Mathf.PI / 4));
        }
    }


    void StartAttack()
    {
        if (mode == eMode.attack)
        {
            audioSource.clip = SoundBank.instance?.Hit_Hero;
            if(!audioSource.isPlaying)
                audioSource.Play();

            animCancelable = false;

            atkHitBox.transform.localRotation = Quaternion.Euler(45, 0, 45 * facing);
            //atkHitBox.transform.localPosition = Vector3.ProjectOnPlane(atkHitBox.transform.localPosition, Vector3.up) + 0.5f*Vector3.up;
            atkHitBox.SetActive(true);
        }
    }

    void StopAttack()
    {
        atkStepVel = Vector3.zero;
        atkHitBox.SetActive(false);
        animCancelable = true;
        if (!nextAtk)
        {
            mode = eMode.idle;
            atkCounter = 0;
        }
        else
        {
            atkCounter++;
            nextAtk = false;
            Vector3 viewDirection = viewDirectionController.viewDirection;
            facing = CalcFacing(viewDirection);
        }
    }

    public void StartRest()
    {
        mode = eMode.rest;
        anim.CrossFade("Hero_StartRest_" + facing, 0);
        anim.speed = 1;
        rigid.velocity = Vector3.zero;
        BonfireCamera.instance.ShowMenu();
    }
    void ContinueRest()
    {
        health = maxHealth;
        concentration = 0;
        anim.CrossFade("Hero_Rest_" + facing, 0);
        anim.speed = 1;
        audioSource.clip = SoundBank.instance?.Heal;
        audioSource.Play();
    }
    public void EndRest()
    {
        anim.CrossFade("Hero_EndRest_" + facing, 0);
        anim.speed = 1;
        BonfireCamera.instance.HideMenu();
    }

    void EndRestAnimation()
    {
        mode = eMode.idle;
    }

    void StartHeal()
    {
        flaskCount--;
        health += maxHealth / 2;
        audioSource.clip = SoundBank.instance?.Heal;
        audioSource.Play();
    }

    void StopHeal()
    {
        animCancelable = true;
        mode = eMode.idle;
    }

    void ExitKnockback()
    {
        if(concentration >= maxConcentration)
            concentration = 0;
        mode = eMode.idle;
    }

    void PauseAnimation()
    {
        anim.speed = 0;
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

    public HeroStats GetStats()
    {
        return stats;
    }

}
