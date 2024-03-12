using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = ("AI/States/Attack"))]
public class AttackState : AIState
{
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    [Header("Combo continues chance")]
    [SerializeField] protected int comboContinuesChance = 25;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCombatManager.currentTarget is null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        if (aiCharacter.aiCombatManager.currentTarget.isDead)
            return SwitchState(aiCharacter, aiCharacter.idleState);


        aiCharacter.aiCombatManager.RotateTowardsTargetWhileAttack(aiCharacter);

        aiCharacter.animatorManager.UpdateAnimatorMovementParameters(0, 0);

        if (willPerformCombo && !hasPerformedCombo)
        {
            if (currentAttack.comboActionTrigger is not null)
            {
                PerformCombo(aiCharacter);
            }
        }

        if (aiCharacter.isPerformingAction)
            return this;

        if (!hasPerformedAttack)
        {
            if (aiCharacter.aiCombatManager.actionRecoveryTimer > 0)
            {
                return this;
            }

            PerformAttack(aiCharacter);

            return this;
        }

        if (pivotAfterAttack)
        {
            aiCharacter.aiCombatManager.PivotTowardsTarget(aiCharacter);
        }

        return SwitchState(aiCharacter, aiCharacter.combatStanceState);
    }

    protected virtual void PerformAttack(AICharacterManager aiCharacter)
    {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);
        aiCharacter.aiCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected virtual void PerformCombo(AICharacterManager aiCharacter)
    {
        if (!aiCharacter.aiCombatManager.canCombo)
            return;
        if(!WorldUtilityManager.RollForOutcomeChance(comboContinuesChance))
            hasPerformedCombo = true;
        aiCharacter.isPerformingAction = true;
        currentAttack.AttemptToPerformAction(aiCharacter, true);
        aiCharacter.aiCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);
        hasPerformedAttack = false;
        hasPerformedCombo = false;
    }

}
