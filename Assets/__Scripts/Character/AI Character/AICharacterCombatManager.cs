using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AICharacterCombatManager : CombatManager
{
    private AICharacterManager aiCharacter;

    [Header("Action Recovery")]
    public float actionRecoveryTimer = 0;

    [Header("Target Information")]
    public float viewableAngle;
    public Vector3 targetsDirection;
    public float distanceFromTarget;

    [Header("Attack Information")]
    public float attackRotationSpeed = 25;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
        OnBlock += (blockedCharacter) => actionRecoveryTimer -= 0.5f;
        OnParry += (parriedCharacter) => actionRecoveryTimer -= 1;
    }

    protected override void Update()
    {
        base.Update();
        HandleActionRecovery();
    }

    private void FixedUpdate()
    {
        if(currentTarget is not null)
        {
            targetsDirection = currentTarget.transform.position - transform.position;
            viewableAngle = WorldUtilityManager.GetAngleOfTarget(aiCharacter.aiLocomotionManager.GetForward(), targetsDirection);
            distanceFromTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        }
    }

    public void FindTarget(AICharacterManager aiCharacter)
    {
        currentTarget = PlayersInGameManager.instance.playerList.FirstOrDefault();
        
        PivotTowardsTarget(aiCharacter);
    }

    public void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return;

        int p = 0;
        if (viewableAngle >= 20)
            p = 1;
        else if (viewableAngle <= -20)
            p = -1;
        aiCharacter.animatorManager.UpdateAnimatorRotationParameters(p);
        if(Mathf.Abs(p) > 0)
        {
            aiCharacter.animatorManager.UpdateAnimatorMovementParameters(0, 0);
        }
    }

    public void RotateTowardsTargetWhileAttack(AICharacterManager aiCharacter)
    {
        if (currentTarget is null)
            return;

        if (!aiCharacter.canRotate)
            return;

        if (!aiCharacter.isPerformingAction)
            return;

        Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero)
        {
            targetDirection = aiCharacter.aiLocomotionManager.GetForward();
        }

        Quaternion targetRotation = Quaternion.FromToRotation(aiCharacter.aiLocomotionManager.GetForward(), aiCharacter.transform.forward) * Quaternion.LookRotation(targetDirection);
        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
    }


    public void HandleActionRecovery()
    {
        if (actionRecoveryTimer > 0)
        {
            if(!aiCharacter.isPerformingAction)
                actionRecoveryTimer -= Time.deltaTime;
        }
    }

}
