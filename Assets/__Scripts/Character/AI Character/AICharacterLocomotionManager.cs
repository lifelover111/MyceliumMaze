using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterLocomotionManager : LocomotionManager
{
    private AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();
    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if(aiCharacter.isMoving)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation * Quaternion.FromToRotation(GetForward(), transform.forward);
        }
    }

    public void RotateTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isMoving)
        {
            aiCharacter.transform.rotation = Quaternion.FromToRotation(GetForward(), aiCharacter.aiCombatManager.targetsDirection);
        }
    }

    public void MoveToTheSide(AICharacterManager aiCharacter, Vector3 moveDirection)
    {
        if(aiCharacter.isMoving)
        {
            aiCharacter.animatorManager.UpdateAnimatorMovementParameters(moveDirection.x, moveDirection.z, 0.05f);
        }
    }

}
