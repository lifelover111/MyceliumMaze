using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Surround Target")]
public class SurroundState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;

        if (aiCharacter.aiCombatManager.currentTarget is null)
            return SwitchState(aiCharacter, aiCharacter.surroundState);

        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.navMeshAgent.transform.right * 7.5f, path);
        aiCharacter.navMeshAgent.SetPath(path);
        int a = 1;
        if(path.status == NavMeshPathStatus.PathComplete || path.status == NavMeshPathStatus.PathComplete)
        {
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.navMeshAgent.transform.right * -7.5f, path);
            aiCharacter.navMeshAgent.SetPath(path);
            a = -1;
        }

        aiCharacter.aiLocomotionManager.RotateTowardsTarget(aiCharacter);

        if (aiCharacter.isMoving)
        {
            aiCharacter.animatorManager.UpdateAnimatorMovementParameters(1*a, 0);
        }
        else
        {
            aiCharacter.animatorManager.UpdateAnimatorMovementParameters(0, 0);
        }

        return this;
    }

}
