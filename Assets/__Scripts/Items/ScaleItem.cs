using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive/Scale")]
public class ScaleItem : PassiveItem
{
    [Header("Item Properties")]
    [SerializeField] private float scaleValue;

    public override void PickUp(PlayerManager player)
    {
        base.PickUp(player);
        player.transform.localScale = Vector3.one * (player.transform.localScale.x + scaleValue);
        player.playerLocomotionManager.speedForward += scaleValue;
        player.playerLocomotionManager.speedToSide += scaleValue;
        player.playerLocomotionManager.speedBackward += scaleValue;
    }

    public override void Remove(PlayerManager player)
    {
        base.Remove(player);
        player.transform.localScale = Vector3.one * (player.transform.localScale.x - scaleValue);
        player.playerLocomotionManager.speedForward -= scaleValue;
        player.playerLocomotionManager.speedToSide -= scaleValue;
        player.playerLocomotionManager.speedBackward -= scaleValue;
    }
}
