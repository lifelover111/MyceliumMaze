using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Items/Active/Debug")]
public class DebugItem : Item
{
    public override void PickUp(PlayerManager player)
    {
        base.PickUp(player);
    }

    public override void Remove(PlayerManager player)
    {
        base.Remove(player);
    }

    public override void Use(PlayerManager player)
    {
        base.Use(player);
        Debug.Log("Item used by " + player);
    }
}