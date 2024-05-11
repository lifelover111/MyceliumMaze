using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    protected CharacterManager character;

    public CharacterManager currentTarget;

    private MeleeWeaponCollider weaponDamageCollider;
    private RangeWeapon rangeWeapon;

    public bool canCombo = false;
    public bool canParry = false;
    public MeleeWeaponCollider Weapon => weaponDamageCollider;

    [Header("Damage")]
    public float commonPhysicalDamage;
    public float commonMentalDamage;
    public float commonConcentrationDamage;


    public event System.Action<CharacterManager> OnParry;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        if (character.weapon.TryGetComponent(out MeleeWeaponCollider damageCollider))
        {
            weaponDamageCollider = damageCollider;
            SetDamage(commonPhysicalDamage, commonMentalDamage);
            SetConcentrationDamage(commonConcentrationDamage);
        }
        if (character.weapon.TryGetComponent(out RangeWeapon rangeWeapon))
            this.rangeWeapon = rangeWeapon;
    }

    protected virtual void Start()
    {
        DisableWeaponCollider();
        character.OnDead += DisableWeaponCollider;
        character.OnDead += DisableDamagableColliders;
    }

    protected virtual void Update()
    {
        HandleBlock();
    }

    public virtual void TryAttack()
    {
        if (character.isPerformingAction)
        {
            if(canCombo)
            {
                character.animatorManager.PlayComboAnimation("Attack", true, true);
                character.isPerformingAction = true;
                character.canMove = false;
                character.canRotate = true;
            }
            return;
        }
        character.animatorManager.PlayTargetAttackAnimation(character.animationKeys.Attack, true, true);
        character.canMove = false;
        character.canRotate = true;
    }

    public void TryBlock()
    {
        //if (!character.canBlockAttacks)
        //    return;

        //character.animatorManager.UpdateAnimatorBlockParameters(PlayerInputManager.instance.blockInput);
    }

    protected virtual void HandleBlock()
    {
        if (!character.canBlockAttacks)
            return;

        if (character.isBlocking)
            character.statsManager.RegenerateConcentration(2, 0.5f, true);
    }


    public void BlockDamage(CharacterManager weaponOwner, DamageCollider damageCollider)
    {
        TakeConcentrationDamageEffect concentrationDamageEffect = Instantiate(WorldCharacterEffectManager.instance.concentrationDamageEffect);

        if (character.combatManager.canParry)
        {
            if (SoundBank.instance.parrySound != null)
                character.combatManager.Weapon.soundManager.PlaySound(SoundBank.instance.parrySound);

            concentrationDamageEffect.concentrationDamage = damageCollider.concentrationDamage * damageCollider.concentrationDamageBlockMultiplier;
            concentrationDamageEffect.characterCausingDamage = character;
            var parryEffect = Instantiate(WorldEffectsManager.instance.parryEffectPrefab);
            parryEffect.transform.position = character.weapon.transform.position;
            parryEffect.transform.localPosition += 0.35f * Vector3.left;
            weaponOwner.animatorManager.DisableWeaponSlash();
            weaponOwner.effectsManager.ProcessInstantEffect(concentrationDamageEffect);
            character.animatorManager.PlayTargetActionAnimation(character.animationKeys.Parry, true, true);

            if (weaponOwner.statsManager.Concentration >= weaponOwner.statsManager.MaxConcentration)
            {
                weaponOwner.statsManager.OverflowConcentration();
                weaponOwner.animatorManager.CancelAttack();
                weaponOwner.animatorManager.PlayTargetHitAnimation(character.animationKeys.ParriedToStun, true);
            }
            OnParry?.Invoke(weaponOwner);
            return;
        }

        if (SoundBank.instance.blockSounds != null)
            character.combatManager.Weapon.soundManager.PlaySound(SoundBank.instance.blockSounds[Random.Range(0, SoundBank.instance.blockSounds.Length)]);

        var blockEffect = Instantiate(WorldEffectsManager.instance.blockEffectPrefab);
        blockEffect.transform.position = character.weapon.transform.position;
        blockEffect.transform.localPosition += 0.35f * Vector3.left;
        concentrationDamageEffect.concentrationDamage = damageCollider.concentrationDamage * damageCollider.concentrationDamageBlockMultiplier;
        concentrationDamageEffect.characterCausingDamage = weaponOwner;
        character.effectsManager.ProcessInstantEffect(concentrationDamageEffect);
    }

    public void SetDamage(float physicalDamage, float mentalDamage = 0)
    {
        if (weaponDamageCollider != null)
        {
            weaponDamageCollider.physicalDamage = physicalDamage;
            weaponDamageCollider.mentalDamage = mentalDamage;
        }
    }
    public void SetConcentrationDamage(float damage)
    {
        if (weaponDamageCollider != null)
        {
            weaponDamageCollider.concentrationDamage = damage;
        }
    }


    public virtual void DisableDamagableColliders()
    {
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

    public virtual void EnableWeaponCollider()
    {
        weaponDamageCollider.Enable();
    }

    public virtual void DisableWeaponCollider()
    {
        weaponDamageCollider?.Disable();
    }

    public virtual void EnableIsBlocking()
    {
        character.isBlocking = true;
    }

    public virtual void DisableIsBlocking()
    {
        character.isBlocking = false;
    }

    public virtual void EnableCanParry()
    {
        canParry = true;
    }

    public virtual void DisableCanParry() 
    {
        canParry = false;
    }

    public virtual void EnableInvulnerability()
    {
        character.isInvulnerable = true;
    }

    public virtual void DisableInvulnerability()
    {
        character.isInvulnerable = false;
    }

    public virtual void DoRangeAttack()
    {
        rangeWeapon.Attack();
    }

}
