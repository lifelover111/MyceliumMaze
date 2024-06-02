using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/Damaging Parry")]
public class DamagingParryItem : PassiveItem
{
    public float damage;

    private PlayerManager _player { get; set; }

    public override void PickUp(PlayerManager player)
    {
        base.PickUp(player);
        _player = player;
        player.combatManager.OnParry += DamageTarget;
    }

    public override void Remove(PlayerManager player)
    {
        base.Remove(player);
        player.combatManager.OnParry -= DamageTarget;
    }

    public void DamageTarget(CharacterManager character)
    {
        var damageEffect = Instantiate(WorldCharacterEffectManager.instance.damageEffect);
        damageEffect.characterCausingDamage = _player;
        damageEffect.physycalDamage = damage;
        damageEffect.playDamageAnimation = false;
        damageEffect.withBlood = false;
        damageEffect.playStunAnimation = false;
        character.effectsManager.ProcessInstantEffect(damageEffect);
    }
}
