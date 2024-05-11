using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetActionFlags : StateMachineBehaviour
{
    CharacterManager characterManager;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (characterManager is null)
            characterManager = animator.GetComponent<CharacterManager>();

        characterManager.isPerformingAction = false;
        characterManager.canRotate = true;
        characterManager.canMove = true;
        characterManager.isBlocking = false;
        characterManager.locomotionManager.isDashing = false;
        characterManager.isInvulnerable = false;
        characterManager.combatManager.canParry = false;
        characterManager.animatorManager.DisableCanDoCombo();
        characterManager.animatorManager.DisableAttackCollider();
        characterManager.animatorManager.DisableWeaponSlash();
        characterManager.animatorManager.ResetAnimationFlags();

        if (characterManager is PlayerManager player)
        {
            player.playerCombatManager.DisableCanCancelAttack();
            player.itemManager.ClearOffHand();
        }
        //TODO: убрать отсюда
        else if (characterManager is BossCharacterManager boss)
        {
            boss.StopMoving();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
