using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerInputManager inputManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerUIController playerUIController;
    [HideInInspector] public ItemManager itemManager;


    public event System.Action OnInteract;
    public event System.Action<PlayerManager> OnCastSpell = delegate { };
    public event System.Action OnSporeCountChanged = delegate { };

    public bool CanInteract => OnInteract is not null;

    [Header("Player's spores")]
    public int sporeCount = 0;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += (Scene arg0, LoadSceneMode arg1) => OnStart();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        inputManager = GetComponent<PlayerInputManager>();
        statsManager = GetComponent<PlayerStatsManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerUIController = GetComponent<PlayerUIController>();
        itemManager = GetComponent<ItemManager>();

    }

    protected override void Start()
    {
        base.Start();
        SubscribeToInputEvents();
        OnStart();
    }

    protected override void Update()
    {
        base.Update();
        playerLocomotionManager.HandleAllMovement();
    }

    private void OnStart()
    {
        characterController.enabled = false;
        transform.position = Vector3.zero;
        characterController.enabled = true;
        CameraPivot.instance.target = transform;
        playerStatsManager.Concentration = 0;
    }

    public void CastSpell()
    {
        OnCastSpell?.Invoke(this);
    }

    public PurchaseUI GetPurchaseUI()
    {
        return playerUIController.purchaseWindow;
    }

    public void GiveSpores(int sporeCount)
    {
        this.sporeCount += sporeCount;
        OnSporeCountChanged?.Invoke();
    }

    public bool TryTakeSpores(int sporeCount)
    {
        if(sporeCount > this.sporeCount)
            return false;

        this.sporeCount -= sporeCount;
        return true;
    }

    public void UnsubscribeCastEvent()
    {
        OnCastSpell = delegate { };
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
