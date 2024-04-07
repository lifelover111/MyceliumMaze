using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAttackFlags : StateMachineBehaviour
{
    CharacterManager characterManager;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (characterManager is null)
        //    characterManager = animator.GetComponent<CharacterManager>();

        //characterManager.animatorManager.DisableCanDoCombo();
        //characterManager.animatorManager.DisableAttackCollider();
        //characterManager.animatorManager.DisableWeaponSlash();

        //if (characterManager is PlayerManager player)
        //{
        //    player.playerAnimatorManager.DisableCanDoCombo();
        //    player.playerAnimatorManager.DisableAttackCollider();
        //    player.playerAnimatorManager.DisableWeaponSlash();
        //    player.playerCombatManager.DisableCanCancelAttack();
        //}
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

}
