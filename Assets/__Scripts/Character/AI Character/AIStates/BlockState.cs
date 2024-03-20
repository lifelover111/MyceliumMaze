using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "AI/States/Block")]
public class BlockState : AIState
{
    [Header("State Parameters")]
    [SerializeField] private float maxBlockHoldTime = 3;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float minBlockAngle = -25;
    [SerializeField] private float maxBlockAngle = 25;
    [SerializeField] private float tryOpenDelay = 0.5f;
    [SerializeField] private int openChance = 25;

    [Header("State flags")]
    private bool hasPerformedBlock = false;

    private float blockedTime;
    private float triedOpenTime;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        triedOpenTime -= Time.fixedDeltaTime;
        

        if (!aiCharacter.canBlockAttacks)
            return SwitchState(aiCharacter, aiCharacter.idleState);
                
        if (aiCharacter.aiCombatManager.currentTarget is null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        //if (aiCharacter.isPerformingAction)
        //    return this;

        if (aiCharacter.aiCombatManager.viewableAngle < minBlockAngle || aiCharacter.aiCombatManager.viewableAngle > maxBlockAngle)
        {
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);
        }

        //aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation,
        //    Quaternion.FromToRotation(aiCharacter.aiLocomotionManager.GetForward(), aiCharacter.transform.forward) * Quaternion.LookRotation(aiCharacter.aiCombatManager.targetsDirection, Vector3.up),
        //    rotationSpeed * Time.fixedDeltaTime);

        if (!hasPerformedBlock)
        {
            PerformBlock(aiCharacter);
            return this;
        }

        if(Time.time - blockedTime > maxBlockHoldTime)
        {
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);
        }

        

        if(!aiCharacter.aiCombatManager.currentTarget.isPerformingAction && triedOpenTime <= 0)
        {
            triedOpenTime = tryOpenDelay;
            if (WorldUtilityManager.RollForOutcomeChance(openChance))
            {
                return SwitchState(aiCharacter, aiCharacter.combatStanceState);
            }
        }

        return this;
    }

    protected virtual void PerformBlock(AICharacterManager aiCharacter)
    {
        hasPerformedBlock = true;
        blockedTime = Time.time;
        aiCharacter.animatorManager.UpdateAnimatorBlockParameters(true);
        aiCharacter.animatorManager.PlayTargetActionAnimation(aiCharacter.animationKeys.Block, false);
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);
        hasPerformedBlock = false;
        triedOpenTime = 0;
        aiCharacter.animatorManager.UpdateAnimatorBlockParameters(false);
    }
}
