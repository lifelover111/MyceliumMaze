using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "AI/States/Pursue Target")]
public class PursueTargetState : AIState
{
    [Header("State Parameters")]
    [SerializeField] private int switchSurroundChance = 100;
    [SerializeField] private float switchSurroundFrequency = 0.1f;
    [SerializeField] private float checkAlliesRadius = 10;
    [SerializeField] private int minAlliesCount = 1;

    private float trySwitchTimer;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        trySwitchTimer -= Time.fixedDeltaTime;
        if (aiCharacter.isPerformingAction)
            return this;

        if (aiCharacter.aiCombatManager.currentTarget is null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        if(!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if (TryStartToSurround(aiCharacter))
            return SwitchState(aiCharacter, aiCharacter.surroundState);

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        aiCharacter.aiLocomotionManager.RotateTowardsAgent(aiCharacter);

        if (aiCharacter.aiCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);

        if(aiCharacter.isMoving)
        {
            //var steeringDirection = aiCharacter.aiSteering.GetDirection(aiCharacter.aiCombatManager.currentTarget.transform);
            //steeringDirection = (Quaternion.FromToRotation(aiCharacter.aiLocomotionManager.GetForward(), Vector3.forward) * steeringDirection).normalized;
            aiCharacter.animatorManager.UpdateAnimatorMovementParameters(0, 1);
        }
        else
        {
            aiCharacter.animatorManager.UpdateAnimatorMovementParameters(0, 0);
        }

        aiCharacter.aiCombatManager.PivotTowardsTarget(aiCharacter);

        return this;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        aiCharacter.navMeshAgent.enabled = false;
        aiCharacter.isMoving = false;
    }

    private bool TryStartToSurround(AICharacterManager aiCharacter)
    {
        if (trySwitchTimer > 0)
            return false;


        trySwitchTimer = switchSurroundFrequency;
        if (WorldUtilityManager.RollForOutcomeChance(switchSurroundChance))
        {
            var allies = Physics.OverlapSphere(aiCharacter.aiCombatManager.currentTarget.transform.position, checkAlliesRadius);
            if (allies.Where(a => a.tag == aiCharacter.tag).Count() > minAlliesCount)
                return true;
        }

        return false;
    }
}
