using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Active/Speed up forward")]
public class UseSpeedItem : ActiveItem
{
    [Header("Speed forward applier")]
    public float speed;
    public int continuity;

    public override void Reset()
    {
        base.Reset();
        OnTryUse += TryUseSpeed;
    }

    private bool TryUseSpeed(PlayerManager player)
    {
        player.StartCoroutine(SpeedUpCoroutine(player));
        return true;
    }

    private IEnumerator SpeedUpCoroutine(PlayerManager player)
    {
        yield return new WaitUntil(() => player.itemManager.activeItem.currentCooldown == 0);
        player.itemManager.activeItem.currentCooldown = player.itemManager.activeItem.cooldownUnits - 1;
        player.playerLocomotionManager.speedForward += speed;

        float cooldownCounter = 0;
        int cooldownSpeed = player.itemManager.activeItem.cooldownUnits / continuity;
        while (player.itemManager.activeItem.currentCooldown > 0)
        {
            yield return new WaitForFixedUpdate();
            cooldownCounter += Time.fixedDeltaTime;
            if (cooldownCounter >= 1)
            {
                player.itemManager.activeItem.currentCooldown -= cooldownSpeed;

                cooldownCounter = 0;
            }
        }
        player.playerLocomotionManager.speedForward -= speed;
        if (player.itemManager.activeItem.currentCooldown < 0)
            player.itemManager.activeItem.currentCooldown = 0;
    }
}
