using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if(aiCharacter.combatManager.currentTarget is not null)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }
        else
        {
            aiCharacter.aiCombatManager.FindTarget();
            return this;
        }
    }
}
