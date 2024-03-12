using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Surround Target")]
public class SurroundState : AIState
{
    private Vector3 moveDirection;
    private float switchDirectionDelay = 5;
    private float switchDirectionTimer;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        switchDirectionTimer -= Time.fixedDeltaTime;
        if (aiCharacter.isPerformingAction)
            return this;

        if(!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if (aiCharacter.aiCombatManager.currentTarget is null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        moveDirection = DecideDirection(aiCharacter);
        aiCharacter.aiLocomotionManager.MoveToTheSide(aiCharacter, moveDirection);
        aiCharacter.aiLocomotionManager.RotateTowardsAgent(aiCharacter);



        if (aiCharacter.aiCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);

        return this;
    }

    Vector3 DecideDirection(AICharacterManager aiCharacter)
    {
        if (switchDirectionTimer > 0)
            return moveDirection;
        switchDirectionTimer = switchDirectionDelay;
        NavMeshHit hitRight;
        bool right = NavMesh.SamplePosition(aiCharacter.transform.position + Quaternion.Euler(0, -90, 0) * aiCharacter.aiLocomotionManager.GetForward() * 100, out hitRight, 100f, NavMesh.AllAreas);
        NavMeshHit hitLeft;
        bool left = NavMesh.SamplePosition(aiCharacter.transform.position + Quaternion.Euler(0, 90, 0) * aiCharacter.aiLocomotionManager.GetForward() * 100, out hitLeft, 100f, NavMesh.AllAreas);


        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        if (!right || hitLeft.distance > hitRight.distance)
        {
            return Vector3.left;
        }
        else if (!left || hitRight.distance >= hitLeft.distance)
        {
            return Vector3.right;
        }
        else
        {
            return Vector3.forward;
        }
    }

}
