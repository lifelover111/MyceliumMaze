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
            target.combatManager.BlockDamage(weaponOwner, this);
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
            if (target.isInvulnerable)
                return;

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



}
