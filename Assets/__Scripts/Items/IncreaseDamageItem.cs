using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/Increase Damage")]
public class IncreaseDamageItem : PassiveItem
{
    [Header("Modifier")]
    public float modifier = 0.1f;
    public override void PickUp(PlayerManager player)
    {
        base.PickUp(player);
        player.playerStatsManager.damageModifier += modifier;
    }
    public override void Remove(PlayerManager player)
    {
        base.Remove(player);
        player.playerStatsManager.damageModifier -= modifier;
    }
}
