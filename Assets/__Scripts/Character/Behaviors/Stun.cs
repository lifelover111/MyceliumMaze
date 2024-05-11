using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : StateMachineBehaviour
{
    CharacterManager characterManager;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (characterManager is null)
            characterManager = animator.GetComponent<CharacterManager>();
        characterManager.canRotate = false;
        characterManager.canMove = false;
        characterManager.isBlocking = false;
        characterManager.animatorManager.DisableCanDoCombo();
        characterManager.animatorManager.DisableAttackCollider();
        characterManager.animatorManager.DisableWeaponSlash();

        if (characterManager is PlayerManager player)
        {
            player.playerCombatManager.DisableCanCancelAttack();
            player.UnsubscribeCastEvent();
        }
        //TODO: убрать отсюда
        else if(characterManager is BossCharacterManager boss)
        {
            boss.StopMoving();
        }
    }
}
