using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/Speed")]
public class SpeedItem : PassiveItem
{
    [Header("Speed applier")]
    public float speed;

    public override void PickUp(PlayerManager player)
    {
        base.PickUp(player);
        player.playerLocomotionManager.speedForward += speed;
        player.playerLocomotionManager.speedToSide += speed;
        player.playerLocomotionManager.speedBackward += speed;
    }

    public override void Remove(PlayerManager player)
    {
        base.Remove(player);
        player.playerLocomotionManager.speedForward -= speed;
        player.playerLocomotionManager.speedToSide -= speed;
        player.playerLocomotionManager.speedBackward -= speed;
    }
}
