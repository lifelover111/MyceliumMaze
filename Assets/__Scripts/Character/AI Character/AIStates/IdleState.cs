using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isSleeping)
            return this;

        if (aiCharacter.combatManager.currentTarget is not null)
        {
            if (WorldUtilityManager.RollForOutcomeChance(50))
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
            else
                return SwitchState(aiCharacter, aiCharacter.surroundState);
        }
        else
        {
            aiCharacter.aiCombatManager.FindTarget(aiCharacter);
            return this;
        }
    }
}
