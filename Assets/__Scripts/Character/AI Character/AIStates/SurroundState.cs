using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Surround Target")]
public class SurroundState : AIState
{
    private Vector3 moveDirection;
    private float switchDirectionDelay = 0.5f;
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


        aiCharacter.aiLocomotionManager.MoveToTheSide(aiCharacter, moveDirection);
        aiCharacter.aiLocomotionManager.RotateTowardsAgent(aiCharacter);

        if (!TryDecideDirection(aiCharacter, out moveDirection))
        {
            return this;
        }

        if (aiCharacter.aiCombatManager.actionRecoveryTimer <= 0)
        {
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);
        }


        if (aiCharacter.aiCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);

        return this;
    }

    bool TryDecideDirection(AICharacterManager aiCharacter, out Vector3 direction)
    {
        if (switchDirectionTimer > 0)
        {
            direction = moveDirection;
            return false;
        }
        switchDirectionTimer = switchDirectionDelay;
        NavMeshHit hitRight;
        bool right = NavMesh.SamplePosition(aiCharacter.transform.position + Quaternion.Euler(0, -90, 0) * aiCharacter.aiLocomotionManager.GetForward() * 100, out hitRight, 100f, NavMesh.AllAreas);
        NavMeshHit hitLeft;
        bool left = NavMesh.SamplePosition(aiCharacter.transform.position + Quaternion.Euler(0, 90, 0) * aiCharacter.aiLocomotionManager.GetForward() * 100, out hitLeft, 100f, NavMesh.AllAreas);


        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        if (Random.value < 0.25)
        {
            direction = Vector3.back;
            return true;
        }
        if (!right || hitLeft.distance > hitRight.distance)
        {
            direction = Vector3.left;
            return true;
        }
        else if (!left || hitRight.distance >= hitLeft.distance)
        {
            direction = Vector3.right;
            return true;
        }
        else
        {
            direction = Vector3.back;
            return true;
        }
    }

    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);
        switchDirectionTimer = 0;
    }

}
