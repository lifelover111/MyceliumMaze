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
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation * Quaternion.FromToRotation(GetForward(), Vector3.forward);
        }
    }

    public void RotateTowardsTarget(AICharacterManager aiCharacter)
    {
        if(aiCharacter.isMoving)
        {
            aiCharacter.transform.rotation = Quaternion.FromToRotation(aiCharacter.aiLocomotionManager.GetForward(), aiCharacter.aiCombatManager.currentTarget.transform.position - aiCharacter.transform.position);
        }
    }

    public override Vector3 GetForward()
    {
        return aiCharacter.forward;
    }
}
