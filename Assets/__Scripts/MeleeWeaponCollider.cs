using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MeleeWeaponCollider : DamageCollider
{
    private CharacterManager weaponOwner;

    public SoundManager soundManager;

    protected override void Awake()
    {
        base.Awake();
        weaponOwner = GetComponentInParent<CharacterManager>();
        soundManager = GetComponent<SoundManager>();
    }
    protected override void DamageTarget(CharacterManager target, bool withConcentrationDamage = true)
    {
        if (target == weaponOwner || target.CompareTag(weaponOwner.tag)) return;
        if (target.isInvulnerable) return;
        base.DamageTarget(target, withConcentrationDamage);

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

        if (withVFX)
        {
            var takeDamageVFX = Instantiate(WorldEffectsManager.instance.takeDamageEffectPrefab);
            takeDamageVFX.transform.position = contactPoint;
            if (target is PlayerManager)
            {
                takeDamageVFX.GetComponent<CFXR_Effect>().cameraShake.enabled = true;
            }

            if (SoundBank.instance.takeDamageSound != null)
                target.soundManager.PlaySound(SoundBank.instance.takeDamageSound);
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
            if(SoundBank.instance.parrySound != null)
                character.combatManager.Weapon.soundManager.PlaySound(SoundBank.instance.parrySound);

            concentrationDamageEffect.concentrationDamage = concentrationDamage * concentrationDamageBlockMultiplier;
            concentrationDamageEffect.characterCausingDamage = character;
            var parryEffect = Instantiate(WorldEffectsManager.instance.parryEffectPrefab);
            parryEffect.transform.position = character.weapon.transform.position;
            parryEffect.transform.localPosition += 0.35f * Vector3.left;
            weaponOwner.effectsManager.ProcessInstantEffect(concentrationDamageEffect);
            character.animatorManager.PlayTargetActionAnimation(character.animationKeys.Parry, true, true);

            if (weaponOwner.statsManager.Concentration >= weaponOwner.statsManager.MaxConcentration)
            {
                weaponOwner.statsManager.OverflowConcentration();
                weaponOwner.animatorManager.CancelAttack();
                weaponOwner.animatorManager.PlayTargetHitAnimation(character.animationKeys.ParriedToStun, true);
            }

            return;
        }

        if (SoundBank.instance.blockSounds != null)
            character.combatManager.Weapon.soundManager.PlaySound(SoundBank.instance.blockSounds[Random.Range(0, SoundBank.instance.blockSounds.Length)]);

        var blockEffect = Instantiate(WorldEffectsManager.instance.blockEffectPrefab);
        blockEffect.transform.position = character.weapon.transform.position;
        blockEffect.transform.localPosition += 0.35f*Vector3.left;
        concentrationDamageEffect.concentrationDamage = concentrationDamage * concentrationDamageBlockMultiplier;
        concentrationDamageEffect.characterCausingDamage = weaponOwner;
        character.effectsManager.ProcessInstantEffect(concentrationDamageEffect);
    }


}
