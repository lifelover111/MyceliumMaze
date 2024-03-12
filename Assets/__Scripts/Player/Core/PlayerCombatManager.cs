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
        if (character.isPerformingAction) return;

        if (PlayerInputManager.instance.blockInput)
        {
            character.isPerformingAction = true;
            character.canMove = false;
            character.animatorManager.PlayTargetActionAnimation(character.animationKeys.Block, true);
        }
    }
}
