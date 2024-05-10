using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Boss/Phases/Boss 0 second phase")]
public class Boss_0_SecondPhase : BossPhase
{
    [Header("Health threshold to start phase")]
    public float healthThreshold = 0.5f;

    [Header("Animation key")]
    public string key = "PhaseTwo";
    public string quitAnimationTrigger = "_completePhaseTransition";

    [Header("Properties")]
    public AICharacterAttackAction firstConstantAttck;
    public float stunDuration = 5f;
    public List<AICharacterAttackAction> newAttacks;
    public GameObject effectPrefab;

    private BossCharacterManager bossCharacter;

    public override void InitializePhaseListeners(BossCharacterManager boss)
    {
        bossCharacter = boss;
        boss.statsManager.OnHealthChanged += PhaseListener;
    }

    protected override void StartPhase(BossCharacterManager boss)
    {
        if (boss.statsManager.Health > boss.statsManager.MaxHealth * healthThreshold)
            return;

        boss.statsManager.OnHealthChanged -= PhaseListener;

        boss.StartCoroutine(StartPhaseCoroutine(boss));
    }

    private void PhaseListener()
    {
        StartPhase(bossCharacter);
    }

    private IEnumerator StartPhaseCoroutine(BossCharacterManager boss)
    {
        yield return new WaitForEndOfFrame();

        var effect = Instantiate(effectPrefab);

        var pos = boss.transform.position;
        pos.y = 0;
        effect.transform.position = pos;

        boss.playStunAnimation = false;
        boss.animatorManager.PlayTargetActionAnimation(key, true);
        yield return new WaitForSecondsRealtime(stunDuration);
        boss.isPerformingAction = true;
        boss.aiCombatManager.actionRecoveryTimer = 0;
        boss.animator.SetTrigger(quitAnimationTrigger);
        boss.playStunAnimation = true;
        boss.combatStanceState.aiCharacterAttacks.AddRange(newAttacks);
        boss.combatStanceState.ForceSetNextAttackAction(boss, firstConstantAttck, true);
        Destroy(effect);

    }
}
