using UnityEngine;

[CreateAssetMenu(menuName = "CharacterEffects/InstantEffects/Take Concentration Damage")]
public class TakeConcentrationDamageEffect : InstantCharacterEffect
{
    public CharacterManager characterCausingDamage;
    public float concentrationDamage;
    public bool playAnimation = true;

    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);
        if (character.isDead) return;
        if (character.isInvulnerable) return;

        character.statsManager.Concentration += concentrationDamage;
        character.statsManager.Concentration = 
            character.statsManager.Concentration >= character.statsManager.MaxConcentration ? 
            character.statsManager.MaxConcentration 
            : character.statsManager.Concentration;

        if (!playAnimation)
            return;

        if(character.isBlocking)
        {
            if(character.statsManager.Concentration >= character.statsManager.MaxConcentration)
            {
                character.statsManager.OverflowConcentration();
                character.animatorManager.PlayTargetHitAnimation(character.animationKeys.BreakBlock, true);
            }

            else
            {
                character.animatorManager.PlayTargetHitAnimation(character.animationKeys.BlockHit, true);
            }
        }
    }
}
