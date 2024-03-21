using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    PlayerControls playerControls;
    [SerializeField] Vector2 movementInput;
    [SerializeField] public bool autoMoveInput;
    [SerializeField] bool attackInput;
    [SerializeField] public bool blockInput;
    [SerializeField] bool dashInput;
    [SerializeField] bool healInput;
    [SerializeField] bool useItemInput;

    public Vector2 mousePosition;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    public event System.Action OnDash = delegate { };
    public event System.Action OnAttack = delegate { };
    public event System.Action OnHeal = delegate { };
    public event System.Action OnBlockStateChanged = delegate { };
    public event System.Action OnUseItem = delegate { };

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        HandleAllInputs();
    }

    private void OnEnable()
    {
        if (playerControls is null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerRotation.Rotation.performed += i => mousePosition = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.AutoMovement.canceled += i => autoMoveInput = false;
            playerControls.PlayerMovement.AutoMovement.started += i => autoMoveInput = true;
            playerControls.PlayerActions.Attack.performed += i => attackInput = true;
            playerControls.PlayerActions.Block.performed += i => { blockInput = i.ReadValue<float>() > 0.5; OnBlockStateChanged?.Invoke(); };
            playerControls.PlayerActions.Dash.performed += i => dashInput = true;
            playerControls.PlayerActions.Heal.performed += i => healInput = true;
            playerControls.PlayerActions.UseItem.performed += i => useItemInput = true;
        }
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    void HandleAllInputs()
    {
        HandleMovementInput();
        HandleDashInput();
        HandleAttackInput();
        HandleHealInput();
        HandleUseItemInput();
    }
    
    void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
    }

    void HandleDashInput()
    {
        if(dashInput)
        {
            dashInput = false;
            //TODO: return do nothing if UI open
            OnDash?.Invoke();
        }
    }

    void HandleAttackInput()
    {
        if (attackInput)
        {
            attackInput = false;
            //TODO: return do nothing if UI open
            OnAttack?.Invoke();
        }
    }

    void HandleHealInput()
    {
        if (healInput)
        {
            healInput = false;
            //TODO: return do nothing if UI open
            OnHeal?.Invoke();
        }
    }

    void HandleUseItemInput()
    {
        if (useItemInput)
        {
            useItemInput = false;
            //TODO: return do nothing if UI open
            OnUseItem?.Invoke();
        }
    }
}
