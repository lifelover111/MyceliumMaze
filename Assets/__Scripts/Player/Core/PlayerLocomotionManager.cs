using MBS.Controller.Scene;
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
    
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    private Vector3 dashDirection;

    [SerializeField] float speedForward;
    [SerializeField] float speedBackward;
    [SerializeField] float speedToSide;
    [SerializeField] float rotationSpeed;
    private Vector3 playerRight => player.transform.forward;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void HandleAllMovement()
    {
        //if (player.isPerformingAction)
        //    return;

        HandleRotation();
        if (!PlayerInputManager.instance.autoMoveInput)
            HandleWalk();
        else
            HandleHoldWalkButtonWalk();
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
        moveDirection = Camera.main.transform.forward * verticalMovement;
        moveDirection += Camera.main.transform.right * horizontalMovement;
        moveDirection.y = 0;
        moveDirection.Normalize();
        player.characterController.Move(moveDirection * GetSpeed(moveDirection) * Time.deltaTime);


        Vector3 viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(PlayerInputManager.instance.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection.y = 0;
        viewDirection.Normalize();
        float angle = Vector2.SignedAngle(new Vector2(viewDirection.x, viewDirection.z), Vector2.up);
        Vector3 walkTree = (Quaternion.AngleAxis(angle, Vector3.down) * moveDirection).normalized;
        player.animatorManager.UpdateAnimatorMovementParameters(walkTree.x, walkTree.z);
    }

    private void HandleHoldWalkButtonWalk()
    {
        if (!player.canMove)
            return;

        moveDirection = GetForward();//Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(PlayerInputManager.instance.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        moveDirection.y = 0;
        moveDirection.Normalize();
        player.characterController.Move(moveDirection * GetSpeed(moveDirection) * Time.deltaTime);

        Vector3 viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(PlayerInputManager.instance.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection.y = 0;
        viewDirection.Normalize();
        float angle = Vector2.SignedAngle(new Vector2(viewDirection.x, viewDirection.z), Vector2.up);
        Vector3 walkTree = (Quaternion.AngleAxis(angle, Vector3.down) * moveDirection).normalized;
        player.animatorManager.UpdateAnimatorMovementParameters(walkTree.x, walkTree.z);

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

        Quaternion newRotation = Quaternion.LookRotation(Quaternion.Euler(0, 90, 0) * targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;

        float p = Mathf.Sin(Mathf.Deg2Rad * Vector3.SignedAngle(transform.rotation * Vector3.left, targetRotationDirection, Vector3.up));
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
        player.animatorManager.UpdateAnimatorMovementParameters(intTree.x, intTree.y);
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
