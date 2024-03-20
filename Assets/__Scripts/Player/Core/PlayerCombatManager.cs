using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : CombatManager
{

    protected override void Update()
    {
        base.Update();
    }
    protected override void HandleBlock()
    {
        base.HandleBlock();
        if (character.isPerformingAction) return;

        if (PlayerInputManager.instance.blockInput)
        {
            character.canMove = false;
            character.animatorManager.PlayTargetActionAnimation(character.animationKeys.Block, true);
        }
    }
}
