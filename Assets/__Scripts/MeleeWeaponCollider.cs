using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponCollider : DamageCollider
{
    private CharacterManager weaponOwner;

    protected override void Awake()
    {
        base.Awake();
        weaponOwner = GetComponentInParent<CharacterManager>();
    }
    protected override void DamageTarget(CharacterManager target, bool withConcentrationDamage = true)
    {
        if (target == weaponOwner || target.CompareTag(weaponOwner.tag)) return;
        //base.DamageTarget(target, withConcentrationDamage);

        if (charactersDamaged.Contains(target)) return;
        charactersDamaged.Add(target);

        var angleHitFrom = Vector3.SignedAngle(weaponOwner.transform.position - target.transform.position, target.locomotionManager.GetForward(), Vector3.up);

        if (target.isBlocking && angleHitFrom < target.maxBlockAngle && angleHitFrom > target.minBlockAngle)
        {
            BlockDamage(target);
            return;
        }

        if (withConcentrationDamage)
        {
            TakeConcentrationDamageEffect concentrationDamageEffect = Instantiate(WorldCharacterEffectManager.instance.concentrationDamageEffect);
            concentrationDamageEffect.concentrationDamage = concentrationDamage;
            concentrationDamageEffect.characterCausingDamage = weaponOwner;
            target.effectsManager.ProcessInstantEffect(concentrationDamageEffect);
        }

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.damageEffect);
        damageEffect.physycalDamage = physicalDamage;
        damageEffect.mentalDamage = mentalDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.characterCausingDamage = weaponOwner;
        target.effectsManager.ProcessInstantEffect(damageEffect);
    }

    protected virtual void BlockDamage(CharacterManager character)
    {
        TakeConcentrationDamageEffect concentrationDamageEffect = Instantiate(WorldCharacterEffectManager.instance.concentrationDamageEffect);

        if (character.combatManager.canParry)
        {
            concentrationDamageEffect.concentrationDamage = concentrationDamage * concentrationDamageBlockMultiplier;
            concentrationDamageEffect.characterCausingDamage = character;
            weaponOwner.effectsManager.ProcessInstantEffect(concentrationDamageEffect);

            if (weaponOwner.statsManager.Concentration >= weaponOwner.statsManager.MaxConcentration)
            {
                weaponOwner.statsManager.OverflowConcentration();
                weaponOwner.animatorManager.PlayTargetHitAnimation(character.animationKeys.ParriedToStun, true);
            }

            return;
        }

        concentrationDamageEffect.concentrationDamage = concentrationDamage * concentrationDamageBlockMultiplier;
        concentrationDamageEffect.characterCausingDamage = weaponOwner;
        character.effectsManager.ProcessInstantEffect(concentrationDamageEffect);
    }


}
