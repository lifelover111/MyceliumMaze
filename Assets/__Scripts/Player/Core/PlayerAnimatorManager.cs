using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimatorManager : AnimatorManager
{
    private PlayerManager player;

    private bool isPlayingStepSound = false;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    #region Animation events
    public void StartFlaskHealing()
    {
        player.playerStatsManager.Heal(player.playerStatsManager.healAmount, 1);
    }

    public void EnableCanCancelAttack()
    {
        player.playerCombatManager.canCancelAttack = true;
    }
    public void DisableCanCancelAttack()
    {
        player.playerCombatManager.canCancelAttack = false;
    }

    public void TryCastSpell()
    {
        player.CastSpell();
    }

    public void PlayStepSound()
    {
        if (player.isPerformingAction)
            return;

        if (SoundBank.instance.playerStepSounds != null && !isPlayingStepSound)
        {
            player.soundManager.PlaySound(SoundBank.instance.playerStepSounds[Random.Range(0, SoundBank.instance.playerStepSounds.Length)]);
            SwitchStepSoundFlag();
        }
    }

    #endregion

    public async void SwitchStepSoundFlag()
    {
        isPlayingStepSound = true;
        await Task.Delay(150);
        isPlayingStepSound = false;
    }
}
