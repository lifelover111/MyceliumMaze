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

    public bool isDead = false;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool isInvulnerable = false;

    [Header("Animation Keys")]
    public AnimatorManager.AnimationKeys animationKeys = new();

    [Header("Weapon")]
    public GameObject weapon;


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
        canMove = false;
        canRotate = false;
        if(!manuallySelectDeathAnimation)
        {
            animatorManager.PlayTargetActionAnimation(animationKeys.Dead, true);
        }

        yield return new WaitForSeconds(5);
    }

    public virtual IEnumerator ProcessConcentrationOverflowEvent()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        statsManager.Concentration = 0;
    }

    protected virtual void IgnoreMyOwnColliders()
    {
        var colliderControllerCollider = GetComponent<Collider>();
        var damagableCharacterColliders = GetComponentsInChildren<Collider>().Where(c => c.gameObject.tag != "Weapon");
        var ignoreColliders = damagableCharacterColliders.Union(new List<Collider>() { colliderControllerCollider });

        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }
}

