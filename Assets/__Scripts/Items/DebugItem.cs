using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Items/Active/Debug")]
public class DebugItem : ActiveItem
{

    public override void Reset()
    {
        base.Reset();
        OnTryUse += UseDebug;
    }

    public bool UseDebug(PlayerManager player)
    {
        Debug.Log("Item used by " + player);
        return true;
    }
}
