using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : AnimatorManager
{
    private PlayerManager player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    #region Animation events
    public void StartFlaskHealing()
    {
        player.playerStatsManager.Heal(player.playerStatsManager.healAmount, 1);
    }

    #endregion

}
