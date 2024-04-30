using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public StatsManager statsManager;
    [HideInInspector] public EffectsManager effectsManager;
    [HideInInspector] public AnimatorManager animatorManager;
    [HideInInspector] public CombatManager combatManager;
    [HideInInspector] public LocomotionManager locomotionManager;

    [Header("Model Forward")]
    public Vector3 forward;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool isInvulnerable = false;
    public bool isDead = false;
    public bool isBlocking = false;

    [Header("Animation Keys")]
    public AnimatorManager.AnimationKeys animationKeys = new();

    [Header("Weapon")]
    public GameObject weapon;

    [Header("Block")]
    public bool canBlockAttacks = true;
    public float minBlockAngle = -45;
    public float maxBlockAngle = 45;

    public event System.Action OnDead = delegate { };

    [Header("Colliders")]
    private Collider[] ownColliders;


    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
        statsManager = GetComponent<StatsManager>();
        effectsManager = GetComponent<EffectsManager>();
        animatorManager = GetComponent<AnimatorManager>();
        animator = GetComponent<Animator>();
        combatManager = GetComponent<CombatManager>();
        locomotionManager = GetComponent<LocomotionManager>();
    }

    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
    }

    protected virtual void Update()
    {
        statsManager.RegenerateConcentration();
    }

    protected virtual void FixedUpdate()
    {

    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        isDead = true;
        canMove = false;
        canRotate = false;
        animatorManager.DisableAttackCollider();
        animatorManager.DisableWeaponSlash();
        animatorManager.CancelAttack();
        OnDead?.Invoke();
        if(!manuallySelectDeathAnimation)
        {
            yield return new WaitForEndOfFrame();
            animatorManager.PlayTargetActionAnimation(animationKeys.Dead, true);
        }

        yield return new WaitForSeconds(5);
    
        if(this is not PlayerManager)
            Destroy(gameObject);
    }

    public virtual IEnumerator ProcessConcentrationOverflowEvent()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        statsManager.Concentration = 0;
    }

    public void DisableColliders()
    {
        foreach(var collider in ownColliders)
            collider.enabled = false;
    }

    public void EnableColliders()
    {
        foreach (var collider in ownColliders)
            collider.enabled = true;
    }

    protected virtual void IgnoreMyOwnColliders()
    {
        var colliderControllerCollider = GetComponent<Collider>();
        var damagableCharacterColliders = GetComponentsInChildren<Collider>().Where(c => c.gameObject.tag != "Weapon");
        var colliders = damagableCharacterColliders.Union(new List<Collider>() { colliderControllerCollider }).ToArray();
        ownColliders = damagableCharacterColliders.ToArray();

        foreach (var collider in colliders)
        {
            foreach (var otherCollider in colliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }

}

