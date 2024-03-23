using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerInputManager inputManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public ItemManager itemManager;

    public event System.Action OnInteract = delegate { };

    protected override void Awake()
    {
        base.Awake();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        inputManager = GetComponent<PlayerInputManager>();
        statsManager = GetComponent<PlayerStatsManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        itemManager = GetComponent<ItemManager>();
    }

    protected override void Start()
    {
        base.Start();
        SubscribeToInputEvents();
    }

    protected override void Update()
    {
        base.Update();
        playerLocomotionManager.HandleAllMovement();
    }

    void SubscribeToInputEvents()
    {
        PlayerInputManager.instance.OnDash += playerLocomotionManager.TryDash;
        PlayerInputManager.instance.OnAttack += playerCombatManager.TryAttack;
        PlayerInputManager.instance.OnHeal += playerStatsManager.TryHeal;
        PlayerInputManager.instance.OnBlockStateChanged += playerCombatManager.TryBlock;
        PlayerInputManager.instance.OnUseItem += itemManager.TryUseItem;
        PlayerInputManager.instance.OnInteract += TryInteract;
    }

    void TryInteract()
    {
        if (isPerformingAction)
            return;

        OnInteract?.Invoke();
    }
}
