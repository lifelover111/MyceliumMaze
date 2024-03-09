using UnityEngine;

[CreateAssetMenu(menuName = "CharacterEffects/InstantEffects/Take Concentration Damage")]
public class TakeConcentrationDamageEffect : InstantCharacterEffect
{
    public float concentrationDamage;
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
    }
}
