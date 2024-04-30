using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Combat Stance")]
public class CombatStanceState : AIState
{
    [Header("Attacks")]
    public List<AICharacterAttackAction> aiCharacterAttacks;
    protected List<AICharacterAttackAction> potentialAttacks;
    private AICharacterAttackAction choosenAttack;
    private AICharacterAttackAction previousAttack;
    protected bool hasAttack = false;


    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToPerformCombo = 25;
    protected bool hasRolledForComboChance = false;

    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 8;
    [SerializeField] public float minimumEngagementDistance = 0;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;

        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if (!aiCharacter.isMoving)
        {
            if (aiCharacter.aiCombatManager.viewableAngle < -30 || aiCharacter.aiCombatManager.viewableAngle > 30)
            {
                aiCharacter.aiCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        aiCharacter.aiLocomotionManager.RotateTowardsAgent(aiCharacter);

        if(aiCharacter.aiCombatManager.currentTarget is null)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }


        if (aiCharacter.aiCombatManager.distanceFromTarget > maximumEngagementDistance)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }

        if (aiCharacter.aiCombatManager.actionRecoveryTimer > 0)
        {
            if(WorldUtilityManager.RollForOutcomeChance(50))
                return SwitchState(aiCharacter, aiCharacter.surroundState);
            else
                return SwitchState(aiCharacter, aiCharacter.blockState);
        }

        if (!hasAttack)
        {
            GetNewAttack(aiCharacter);
        }

        else
        {
            aiCharacter.attackState.currentAttack = choosenAttack;

            return SwitchState(aiCharacter, aiCharacter.attackState);
        }

        if (aiCharacter.aiCombatManager.distanceFromTarget <= minimumEngagementDistance)
        {
            return SwitchState(aiCharacter, aiCharacter.keepDistanceState);
        }

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAction>();
        foreach (var attack in aiCharacterAttacks)
        {
            if (attack.minAttackDistance > aiCharacter.aiCombatManager.distanceFromTarget)
                continue;
            if (attack.maxAttackDistance < aiCharacter.aiCombatManager.distanceFromTarget)
                continue;
            if (attack.minAttackAngle > aiCharacter.aiCombatManager.viewableAngle)
                continue;
            if (attack.maxAttackAngle < aiCharacter.aiCombatManager.viewableAngle)
                continue;

            potentialAttacks.Add(attack);                
        }

        var totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach(var attack in potentialAttacks) 
        {
            processedWeight += attack.attackWeight;
            if(randomWeightValue <= processedWeight)
            {
                previousAttack = choosenAttack;
                choosenAttack = attack;
                hasAttack = true;
                if (canPerformCombo && WorldUtilityManager.RollForOutcomeChance(chanceToPerformCombo))
                    aiCharacter.attackState.willPerformCombo = true;
                else
                    aiCharacter.attackState.willPerformCombo = false;
                return;
            }
        }
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        hasRolledForComboChance = false;
        hasAttack = false;
        aiCharacter.animatorManager.SetAnimatorRotationParameters(0);
    }
}
