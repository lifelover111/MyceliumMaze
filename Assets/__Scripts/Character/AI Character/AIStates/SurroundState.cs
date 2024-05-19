using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Surround Target")]
public class SurroundState : AIState
{
    [Header("Search allies radius")]
    public float searchRadius = 6;

    private Vector3 moveDirection;
    private float switchDirectionDelay = 0.5f;
    private float switchDirectionTimer;

    private bool hasDirection = false;

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

        if(!hasDirection)
        {
            TryDecideDirection(aiCharacter, out moveDirection);
            hasDirection = true;
            return this;
        }

        //if (!TryDecideDirection(aiCharacter, out moveDirection))
        //{
        //    return this;
        //}

        if (aiCharacter.aiCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            return SwitchState(aiCharacter, aiCharacter.keepDistanceState);

        if (aiCharacter.aiCombatManager.actionRecoveryTimer <= 0)
        {
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);
        }

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


        var allies = Physics.OverlapSphere(aiCharacter.transform.position, searchRadius, 1 << LayerMask.NameToLayer("Character"))
            .Select(c => c.gameObject).Where(g => g != aiCharacter.gameObject && g.CompareTag("Enemy"));

        if(allies != null && allies.Count() > 0)
        {
            var closestAllyDirection = allies.OrderBy(a => (a.transform.position - aiCharacter.transform.position).magnitude).First().transform.position - aiCharacter.transform.position;
            closestAllyDirection.Normalize();
            direction = -closestAllyDirection;

            NavMeshPath path1 = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path1);
            aiCharacter.navMeshAgent.SetPath(path1);

            return true;
        }

        NavMeshHit hitRight;
        bool right = NavMesh.SamplePosition(aiCharacter.transform.position + Quaternion.Euler(0, -90, 0) * aiCharacter.aiLocomotionManager.GetForward() * 100, out hitRight, 100f, NavMesh.AllAreas);
        NavMeshHit hitLeft;
        bool left = NavMesh.SamplePosition(aiCharacter.transform.position + Quaternion.Euler(0, 90, 0) * aiCharacter.aiLocomotionManager.GetForward() * 100, out hitLeft, 100f, NavMesh.AllAreas);

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        if (Random.value < 0.25)
        {
            direction = -aiCharacter.aiLocomotionManager.GetForward();
            return true;
        }
        if (!right || hitLeft.distance > hitRight.distance)
        {
            direction =  Quaternion.Euler(0, -90, 0)* aiCharacter.aiLocomotionManager.GetForward();
            return true;
        }
        else if (!left || hitRight.distance >= hitLeft.distance)
        {
            direction = Quaternion.Euler(0, 90, 0) * aiCharacter.aiLocomotionManager.GetForward();
            return true;
        }
        else
        {
            direction = -aiCharacter.aiLocomotionManager.GetForward();
            return true;
        }
    }

    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);
        switchDirectionTimer = 0;
        hasDirection = false;
    }

}
