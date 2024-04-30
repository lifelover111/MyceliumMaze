using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;



[CreateAssetMenu(menuName = "CharacterEffects/InstantEffects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;
    [Header("Damage")]
    public float physycalDamage;
    public float mentalDamage;
    [Header("Contact Point")]
    public Vector3 contactPoint;
    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;

    [Header("Blood")]
    public bool withBlood = true;



    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);
        if (character.isDead) return;
        if (character.isInvulnerable) return;

        if(withBlood)
            BloodParticleSystemHandler.Instance.SpawnBlood(new Vector3(character.transform.position.x, 0, character.transform.position.z), (new Vector3(characterCausingDamage.transform.position.x, 0, characterCausingDamage.transform.position.z) - new Vector3(character.transform.position.x, 0, character.transform.position.z)).normalized * -.3f);

        var damageModifier = characterCausingDamage.statsManager.damageModifier;
        character.statsManager.Health -= physycalDamage * damageModifier;
        character.statsManager.Health = character.statsManager.Health > 0 ? character.statsManager.Health : 0;

        

        if (character is PlayerManager player)
        {
            player.playerStatsManager.Sanity -= mentalDamage;
            player.playerStatsManager.Sanity = player.playerStatsManager.Sanity > 0 ? player.playerStatsManager.Sanity : 0;
        }

        if(!character.isDead)
        {
            var angleHitFrom = Vector3.SignedAngle((new Vector3(contactPoint.x, 0, contactPoint.z) - character.transform.position).normalized, character.locomotionManager.GetForward(), Vector3.up);
            if (character.statsManager.Concentration >= character.statsManager.MaxConcentration)
            {
                character.canMove = false;
                character.canRotate = false;

                PlayStunAnimation(character, angleHitFrom);
                character.statsManager.OverflowConcentration();
                return;
            }

            if (playDamageAnimation)
            {
                character.canMove = false;
                character.canRotate = false;
                character.animatorManager.CancelAttack();
                if (angleHitFrom <= 90 && angleHitFrom >= -90)
                {
                    character.animatorManager.PlayTargetHitAnimation(character.animationKeys.HitForward, true);
                }
                else
                {
                    character.animatorManager.PlayTargetHitAnimation(character.animationKeys.HitBack, true);
                }
            }
        }
    }

    private void PlayStunAnimation(CharacterManager character, float angleHitFrom)
    {
        character.animatorManager.CancelAttack();
        if (character is PlayerManager player)
        {
            if (angleHitFrom <= 60 && angleHitFrom > -60)
            {
                player.playerAnimatorManager.PlayTargetHitAnimation(player.animationKeys.StunForward, true);
            }
            else if (angleHitFrom > 60 && angleHitFrom <= 120)
            {
                player.playerAnimatorManager.PlayTargetHitAnimation(player.animationKeys.StunLeft, true);
            }
            else if (angleHitFrom <= -60 && angleHitFrom > -120)
            {
                player.playerAnimatorManager.PlayTargetHitAnimation(player.animationKeys.StunRight, true);
            }
            else
            {
                player.playerAnimatorManager.PlayTargetHitAnimation(player.animationKeys.StunBack, true);
            }

            return;
        }


        if (angleHitFrom <= 90 && angleHitFrom > -90)
        {
            character.animatorManager.PlayTargetHitAnimation(character.animationKeys.StunForward, true);
        }
        else
        {
            character.animatorManager.PlayTargetHitAnimation(character.animationKeys.StunBack, true);
        }

    }


}

