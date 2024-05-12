using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using VolFx;

public class PlayerStatsManager : StatsManager
{
    private PlayerManager player;

    public float healAmount;
    [SerializeField] float _maxSanity;
    [SerializeField] int _maxHealingFlasksCount;
    [SerializeField] float _sanity;
    public int healingFlasksCount;
    public float MaxSanity => _maxSanity;
    public float Sanity { get { return _sanity; } set { _sanity = value; VhsVol._weight.SetValue(new ClampedFloatParameter(1 - (Sanity/MaxSanity),0,1)); if (_sanity <= 0) HandleDeath(); } }

    public event System.Action OnFlaskCountChanged = delegate { };

    protected override void Awake()
    {
        base.Awake();
        Sanity = _maxSanity;
        healingFlasksCount = _maxHealingFlasksCount;
        player = GetComponent<PlayerManager>();
    }

    public override void TryHeal()
    {
        if (character.isPerformingAction) return;
        if (healingFlasksCount < 1) return;
        player.itemManager.AddToHand(Instantiate(player.flaskPrefab));
        character.canMove = false;
        character.canRotate = false;
        player.playerAnimatorManager.PlayTargetActionAnimation(character.animationKeys.Heal, true);
    }

    public override void Heal(float amount, int flaskCost = 0)
    {
        Health += amount;
        healingFlasksCount--;
        OnFlaskCountChanged?.Invoke();
    }

    protected override void HandleDeath()
    {
        if (player.isDead) return;

        player.isDead = true;
        player.StartCoroutine(player.ProcessDeathEvent());
    }

    public void IncreaseFlaskCount()
    {
        _maxHealingFlasksCount++;
        healingFlasksCount++;
        OnFlaskCountChanged?.Invoke();
    }

}
