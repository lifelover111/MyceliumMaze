using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacterManager : AICharacterManager
{
    [Header("Name")]
    public string Name;

    [Header("Play Hit Animation")]
    public bool playHitAnimation = false;
    public bool playStunAnimation = true;

    [Header("Phases")]
    public List<BossPhase> phases;


    protected override void Start()
    {
        base.Start();
        InitializePhaseListeners();
    }

    private void InitializePhaseListeners()
    {
        foreach (BossPhase phase in phases)
        {
            phase.InitializePhaseListeners(this);
        }
    }

    //TODO: Придумать, где это должно быть
    #region Animation Events

    private Coroutine movingCoroutine;

    public void StartMoving()
    {
        movingCoroutine = StartCoroutine(MoveWhileAttackCoroutine());
    }

    public void StopMoving() 
    {
        if (movingCoroutine == null)
            return;
        StopCoroutine(movingCoroutine);
    }

    private IEnumerator MoveWhileAttackCoroutine()
    {
        while (true)
        {
            var direction = aiCombatManager.targetsDirection;
            direction.y = 0;
            direction.Normalize();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 0.75f * Time.deltaTime);
            characterController.Move(aiLocomotionManager.GetForward() * 5f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
}
