using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Windows;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public bool uiIsOpen = false;

    PlayerControls playerControls;
    [SerializeField] Vector2 movementInput;
    [SerializeField] public bool autoMoveInput;
    [SerializeField] bool attackInput;
    [SerializeField] public bool blockInput;
    [SerializeField] bool dashInput;
    [SerializeField] bool healInput;
    [SerializeField] bool useItemInput;
    [SerializeField] bool rightStickInput;
    [SerializeField] bool interactInput;

    private bool mouseInput = true;

    public Vector2 mousePosition;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Parameters")]
    [SerializeField] private float rightStickThreshold = 1000;

    public event System.Action OnDash = delegate { };
    public event System.Action OnAttack = delegate { };
    public event System.Action OnHeal = delegate { };
    public event System.Action OnBlockStateChanged = delegate { };
    public event System.Action OnUseItem = delegate { };
    public event System.Action OnInteract = delegate { };

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
            playerControls.PlayerRotation.Rotation.performed += i =>
            {
                if (uiIsOpen)
                    return;
                if (mouseInput) 
                    mousePosition = i.ReadValue<Vector2>();
            };
            playerControls.PlayerMovement.Movement.performed += i => ReadLeftStickInput(i);
            playerControls.PlayerRotation.Rotation_Gamepad.performed += i => 
            {
                ReadRightStickInput(i);
                mouseInput = false;
            };
            playerControls.PlayerMovement.AutoMovement.canceled += i => autoMoveInput = false;
            playerControls.PlayerMovement.AutoMovement.started += i => autoMoveInput = true;
            playerControls.PlayerActions.Attack.performed += i => attackInput = true;
            playerControls.PlayerActions.Block.performed += i => { blockInput = i.ReadValue<float>() > 0.5; OnBlockStateChanged?.Invoke(); };
            playerControls.PlayerActions.Dash.performed += i => dashInput = true;
            playerControls.PlayerActions.Heal.performed += i => healInput = true;
            playerControls.PlayerActions.UseItem.performed += i => useItemInput = true;
            playerControls.PlayerActions.Interact.performed += i => interactInput = true;
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
        HandleInteractInput();
    }
    
    void HandleMovementInput()
    {
        if (uiIsOpen)
        {
            verticalInput = 0;
            horizontalInput = 0;
            moveAmount = 0;
            return;
        }
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
    }

    void HandleDashInput()
    {
        if(dashInput)
        {
            dashInput = false;
            if (uiIsOpen)
                return;
            OnDash?.Invoke();
        }
    }

    void HandleAttackInput()
    {
        if (attackInput)
        {
            attackInput = false;
            if (uiIsOpen)
                return;
            OnAttack?.Invoke();
        }
    }

    void HandleHealInput()
    {
        if (healInput)
        {
            healInput = false;
            if (uiIsOpen)
                return;
            OnHeal?.Invoke();
        }
    }

    void HandleUseItemInput()
    {
        if (useItemInput)
        {
            useItemInput = false;
            if (uiIsOpen)
                return;
            OnUseItem?.Invoke();
        }
    }

    void HandleInteractInput()
    {
        if(interactInput)
        {
            interactInput = false;
            if (uiIsOpen)
                return;
            OnInteract?.Invoke();
        }
    }

    void ReadLeftStickInput(InputAction.CallbackContext context)
    {
        if (uiIsOpen)
            return;
     
        if (mouseInput)
            return;

        if (context.ReadValue<Vector2>().magnitude < 0.75f)
            return;
        
        var cameraRelativePos = (Vector2)Camera.main.WorldToScreenPoint(transform.position);
        if(!rightStickInput)
        {
            mousePosition = cameraRelativePos + rightStickThreshold * context.ReadValue<Vector2>();
        }
    }

    void ReadRightStickInput(InputAction.CallbackContext context)
    {
        if (uiIsOpen)
            return;

        var input = context.ReadValue<Vector2>();
        if (input.magnitude < 0.25f)
        {
            rightStickInput = false;
            return;
        }
        else
        {
            rightStickInput = true;
            input.Normalize();
            mousePosition = (Vector2)Camera.main.WorldToScreenPoint(transform.position) + rightStickThreshold * input;
        }

    }
}
