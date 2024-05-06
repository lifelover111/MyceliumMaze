using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFlaskAsItem : PassiveItem
{
    public override void PickUp(PlayerManager player)
    {
        base.PickUp(player);
        player.playerStatsManager.IncreaseFlaskCount();
        player.itemManager.RemoveItem(this);
    }
}
