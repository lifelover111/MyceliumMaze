using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/Concentration damage aura")]
public class ConcentartionDamageAuraItem : PassiveItem
{
    [Header("Concentration damage per second")]
    public float concentrationDamage = 1;

    [Header("Aura radius")]
    public float radius = 4;

    private TakeConcentrationDamageEffect effect;

    public override void Reset()
    {
        base.Reset();
        effect = null;
    }

    public override void ProcessPassiveEffect(PlayerManager player)
    {
        base.ProcessPassiveEffect(player);

        if(effect is null)
        {
            effect = Instantiate(WorldCharacterEffectManager.instance.concentrationDamageEffect);
            effect.characterCausingDamage = player;
            effect.concentrationDamage = concentrationDamage*Time.fixedDeltaTime;
            effect.playAnimation = false;
        }

        var colliders = Physics.OverlapSphere(player.transform.position, radius).Where(c => c.gameObject.layer.Equals(LayerMask.NameToLayer("Character")));
        var characters = colliders.Select(c => c.GetComponent<CharacterManager>()).Where(c => c != player);
        
        if (characters.Count() == 0)
            return;

        foreach ( var character in characters ) 
        {
            character.effectsManager.ProcessInstantEffect(effect);
        }
    }
}
