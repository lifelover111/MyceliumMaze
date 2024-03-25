using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Active/ForceItem")]
public class ForceItem : ActiveItem
{
    [Header("Effect Prefab")]
    [SerializeField] GameObject effectPrefab;

    public override void Reset()
    {
        base.Reset();
        OnTryUse += TryUseForce;
    }

    public bool TryUseForce(PlayerManager player)
    {
        if (player.isPerformingAction)
            return false;

        player.canMove = false;
        player.canRotate = false;

        player.playerAnimatorManager.PlayTargetActionAnimation(player.animationKeys.Cast, true);
        player.OnCastSpell += Force;

        return true;
    }

    private void Force(PlayerManager player)
    {
        player.OnCastSpell -= Force;
        var effect = Instantiate(effectPrefab);
        effect.GetComponent<RedHollowControl>().owner = player;
        effect.transform.position = player.itemManager.OffHand.position + 0.5f* player.playerLocomotionManager.GetForward();
        effect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, player.playerLocomotionManager.GetForward());
    }
}
