using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/Increase concentration damage")]
public class IncreaseConcentrationDamageItem : PassiveItem
{
    [Header("Damage modifier")]
    public float damageModifier = 1.5f;

    public override void PickUp(PlayerManager player)
    {
        base.PickUp(player);
        player.playerStatsManager.concentrationDamageModifier *= damageModifier;
    }

    public override void Remove(PlayerManager player)
    {
        base.Remove(player);
        player.playerStatsManager.concentrationDamageModifier /= damageModifier;
    }
}
