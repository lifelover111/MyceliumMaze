using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotionManager : LocomotionManager
{
    PlayerManager player;
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;
    public bool externallyControlled = false;

    
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    private Vector3 dashDirection;

    [SerializeField] float speedForward;
    [SerializeField] float speedBackward;
    [SerializeField] float speedToSide;
    [SerializeField] float rotationSpeed;
    [SerializeField] float movementAcceleration = 10;


    private static float maxSpeed = 12;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void HandleAllMovement()
    {
        //if (player.isPerformingAction)
        //    return;

        if(externallyControlled)
        {
            HandleExternalControlMovement();
            return;
        }

        HandleRotation();
        if (!PlayerInputManager.instance.autoMoveInput)
            HandleWalk();
        else
            HandleHoldWalkButtonWalk();
    }

    public void GoTowards(Vector3 direction)
    {
        player.canMove = false;
        player.canRotate = false;
        moveDirection = direction.normalized;
    }

    void HandleExternalControlMovement()
    {
        if (player.isPerformingAction)
            return;
        player.transform.rotation = Quaternion.FromToRotation(GetForward(), transform.forward) * Quaternion.LookRotation(moveDirection, Vector3.up);
        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, Mathf.Sqrt(speedForward / maxSpeed), false);
        player.characterController.Move(moveDirection * speedForward * Time.deltaTime);
    }

    void GetVerticalAndHorizontalInputs()
    {
        if (PlayerInputManager.instance.autoMoveInput)
        { 
            horizontalMovement = 0f;
            verticalMovement = 0f;
            return; 
        }

        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        verticalMovement = PlayerInputManager.instance.verticalInput;
    }

    private void HandleWalk()
    {
        if (!player.canMove)
            return;

        GetVerticalAndHorizontalInputs();
        var direction = Camera.main.transform.forward * verticalMovement;
        direction += Camera.main.transform.right * horizontalMovement;
        direction.y = 0;
        direction.Normalize();
        moveDirection = Vector3.Slerp(moveDirection, Vector3.ClampMagnitude(direction, 1), movementAcceleration*Time.deltaTime);
        player.characterController.Move(moveDirection * GetSpeed(moveDirection) * Time.deltaTime);


        Vector3 viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(PlayerInputManager.instance.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection.y = 0;
        viewDirection.Normalize();
        float angle = Vector2.SignedAngle(new Vector2(viewDirection.x, viewDirection.z), Vector2.up);
        Vector3 walkTree = Vector3.ClampMagnitude(Quaternion.AngleAxis(angle, Vector3.down) * moveDirection, 1)* Mathf.Sqrt(speedForward/maxSpeed);
        player.animatorManager.UpdateAnimatorMovementParameters(walkTree.x, walkTree.z, false);
    }

    private void HandleHoldWalkButtonWalk()
    {
        if (!player.canMove)
            return;

        var direction = GetForward();
        direction.y = 0;
        direction.Normalize();
        moveDirection = Vector3.Slerp(moveDirection, Vector3.ClampMagnitude(direction, 1), movementAcceleration * Time.deltaTime);
        player.characterController.Move(moveDirection * GetSpeed(moveDirection) * Time.deltaTime);

        Vector3 viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(PlayerInputManager.instance.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection.y = 0;
        viewDirection.Normalize();
        float angle = Vector2.SignedAngle(new Vector2(viewDirection.x, viewDirection.z), Vector2.up);
        Vector3 walkTree = Vector3.ClampMagnitude(Quaternion.AngleAxis(angle, Vector3.down) * moveDirection, 1) * Mathf.Sqrt(speedForward/maxSpeed);
        player.animatorManager.UpdateAnimatorMovementParameters(walkTree.x, walkTree.z, false);

    }

    private void HandleRotation()
    {
        if (!player.canRotate)
            return;

        targetRotationDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(PlayerInputManager.instance.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        targetRotationDirection.y = 0;
        targetRotationDirection.Normalize();

        if(targetRotationDirection == Vector3.zero)
            targetRotationDirection = transform.forward;

        Quaternion newRotation = Quaternion.LookRotation(Quaternion.FromToRotation(GetForward(), transform.forward) * targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;

        float p = Mathf.Cos(Mathf.Deg2Rad * Vector3.SignedAngle(transform.rotation * Vector3.left, targetRotationDirection, Vector3.up));
        player.animatorManager.UpdateAnimatorRotationParameters(p);

    }

    public void TryDash()
    {
        if (player.isPerformingAction)
            return;

        if (PlayerInputManager.instance.moveAmount > 0)
        {
            dashDirection = Camera.main.transform.forward * verticalMovement;
            dashDirection += Camera.main.transform.right * horizontalMovement;
            dashDirection.y = 0;
            dashDirection.Normalize();
        }
        else
        {
            dashDirection = GetForward();
            dashDirection.y = 0;
            dashDirection.Normalize();
        }


        Vector3 viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(PlayerInputManager.instance.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection.y = 0;
        viewDirection.Normalize();
        float angle = Vector2.SignedAngle(new Vector2(viewDirection.x, viewDirection.z), Vector2.up);
        Vector3 inputTree = (Quaternion.AngleAxis(angle, Vector3.down) * dashDirection).normalized;

        Vector2Int intTree = new Vector2Int();
        intTree.x = Mathf.RoundToInt(inputTree.x);
        intTree.y = Mathf.RoundToInt(inputTree.z);

        if(Mathf.Abs(inputTree.x) > Mathf.Abs(inputTree.z))
        {
            intTree.y = 0;
        }
        else
        { 
            intTree.x = 0; 
        }

        player.canRotate = false;
        player.canMove = false;
        player.animatorManager.SetAnimatorMovementParameters(intTree.x, intTree.y);
        player.animatorManager.PlayTargetActionAnimation(player.animationKeys.Dash, true, true);
    }

    private float GetSpeed(Vector3 direction)
    {
        Vector3 viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(PlayerInputManager.instance.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection.y = 0;
        viewDirection.Normalize();

        float angle = Vector3.SignedAngle(viewDirection, direction, Vector3.down);
        if (angle < 0) angle += 360;

        if ((angle >= 0 && angle < 30) || angle > 330)
        {
            return speedForward;
        }
        else if (angle > 150 && angle < 210)
        {
            return speedBackward;
        }
        else
        {
            return speedToSide;
        }
    }
}
