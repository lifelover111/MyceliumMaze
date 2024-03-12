using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Attack")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Combo Action")]
    public string? comboActionTrigger;

    [Header("Action Values")]
    public int attackWeight = 50;
    public float actionRecoveryTime = 1.5f;
    public float minAttackDistance = 0;
    public float maxAttackDistance = 3;
    public float minAttackAngle = -30;
    public float maxAttackAngle = 30;

    public void AttemptToPerformAction(AICharacterManager aiCharacter, bool combo = false)
    {
        if(combo)
        {
            aiCharacter.animatorManager.PlayComboAnimation(comboActionTrigger, true);
            return;
        }
        aiCharacter.animatorManager.PlayTargetAttackAnimation(aiCharacter.animationKeys.Attack, true);
    }
}
