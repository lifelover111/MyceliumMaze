using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Keep Distance")]
public class KeepDistanceState : AIState
{
    [Header("Target Distance")]
    [SerializeField] private float targetDistance;

    [Header("State Settings")]
    [SerializeField] private float noRootMotionSpeed = 3;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;

        if (aiCharacter.aiCombatManager.currentTarget is null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if(aiCharacter.aiCombatManager.distanceFromTarget < aiCharacter.navMeshAgent.stoppingDistance)
            aiCharacter.navMeshAgent.SetDestination(aiCharacter.transform.position + aiCharacter.aiCombatManager.targetsDirection.normalized * aiCharacter.navMeshAgent.stoppingDistance * 1.25f); //aiCharacter.transform.position - aiCharacter.aiLocomotionManager.GetForward()*aiCharacter.navMeshAgent.stoppingDistance*1.25f);
        else
        {
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);
        }


        aiCharacter.aiLocomotionManager.RotateTowardsAgent(aiCharacter);

        if (aiCharacter.aiCombatManager.distanceFromTarget >= targetDistance)
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);

        if (aiCharacter.isMoving)
        {
            if (aiCharacter.noRootMotion)
            {
                var direction = -1 * aiCharacter.aiLocomotionManager.GetForward();
                direction.y = 0;
                direction.Normalize();

                if (!Physics.Raycast(direction, 2 * direction, 3))
                    aiCharacter.characterController.Move(direction * noRootMotionSpeed * Time.deltaTime);
                else
                    aiCharacter.characterController.Move(-1*direction * noRootMotionSpeed * Time.deltaTime);
            }
            aiCharacter.animatorManager.UpdateAnimatorMovementParameters(0, -1);
        }
        else
        {
            aiCharacter.animatorManager.UpdateAnimatorMovementParameters(0, 0);
        }

        aiCharacter.aiCombatManager.PivotTowardsTarget(aiCharacter);

        return this;
    }
}
