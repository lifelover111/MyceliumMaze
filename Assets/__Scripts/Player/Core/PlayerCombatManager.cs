using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : CombatManager
{
    public bool canCancelAttack = false;


    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleBlock()
    {
        base.HandleBlock();
        character.animatorManager.UpdateAnimatorBlockParameters(PlayerInputManager.instance.blockInput);
        if (character.isPerformingAction && !canCancelAttack) return;


        if (PlayerInputManager.instance.blockInput)
        {
            DisableWeaponCollider();
            canCancelAttack = false;
            character.canMove = false;
            character.animatorManager.PlayTargetActionAnimation(character.animationKeys.Block, true);
        }

    }


    public void EnableCanCancelAttack()
    {
        canCancelAttack = true;
    }
    public void DisableCanCancelAttack()
    {
        canCancelAttack = false;
    }

}
