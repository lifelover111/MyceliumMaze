using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamageCollider : DamageCollider
{
    public CharacterManager weaponOwner;
    public bool playDamageAnimation = true;


    [Header("Blockable")]
    public bool blockable = false;

    private string ownerTag;

    private void Start()
    {
        ownerTag = weaponOwner.gameObject.tag;
    }

    protected override void DamageTarget(CharacterManager target, bool withConcentrationDamage = true)
    {
        if (target.CompareTag(ownerTag)) return;
        base.DamageTarget(target, withConcentrationDamage);

        if (charactersDamaged.Contains(target)) return;
        charactersDamaged.Add(target);

        if(blockable)
        {
            var angleHitFrom = Vector3.SignedAngle(weaponOwner.transform.position - target.transform.position, target.locomotionManager.GetForward(), Vector3.up);

            if (target.isBlocking && angleHitFrom < target.maxBlockAngle && angleHitFrom > target.minBlockAngle)
            {
                target.combatManager.BlockDamage(weaponOwner, this);
                return;
            }
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
        }

        if(withSound)
        {
            if (SoundBank.instance.takeDamageSound != null)
                target.soundManager.PlaySound(SoundBank.instance.takeDamageSound);
        }

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.damageEffect);
        damageEffect.playDamageAnimation = playDamageAnimation;
        damageEffect.withBlood = withBlood;
        damageEffect.physycalDamage = physicalDamage;
        damageEffect.mentalDamage = mentalDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.characterCausingDamage = weaponOwner;
        target.effectsManager.ProcessInstantEffect(damageEffect);
    }
}
