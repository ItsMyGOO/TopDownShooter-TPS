using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPSController : MonoBehaviour {
    Transform playerTransform;
    Animator animator;
    Transform cameraTransform;
    CharacterController characterController;

    public enum PlayerPosture {
        Crouch = 0,
        Stand,
        MidAir
    }

    public PlayerPosture playerPosture;

    public enum LocomotionState {
        Idle,
        Walk,
        Run
    }

    public LocomotionState locomotionState = LocomotionState.Idle;

    public enum AimState {
        Normal,
        Aim
    }

    public AimState aimState = AimState.Normal;

    public float crouchSpeed = 1.5f;
    public float walkSpeed = 2.5f;
    public float runSpeed = 5.5f;

    Vector2 moveInput;
    bool isRunning, isCrouch, isAiming, isJumping;

    int postureHash, moveSpeedHash, turnSpeedHash;

    Vector3 playerMovement = Vector3.zero;

    private void Start() {
        playerTransform = transform;
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();

        postureHash = Animator.StringToHash("posture");
        moveSpeedHash = Animator.StringToHash("moveSpeed");
        turnSpeedHash = Animator.StringToHash("turnSpeed");

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        SwitchPlayerState();
        CalculateInputDirection();
        SetupAnimator();
    }

    private void OnAnimatorMove() {
        characterController.Move(animator.deltaPosition);
    }

    #region 玩家输入

    public void GetMoveInput(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    public void GetRunInput(InputAction.CallbackContext context) {
        isRunning = context.ReadValueAsButton();
    }

    public void GetCrouchInput(InputAction.CallbackContext context) {
        isCrouch = context.ReadValueAsButton();
    }

    public void GetAimInput(InputAction.CallbackContext context) {
        isAiming = context.ReadValueAsButton();
    }

    #endregion

    void SwitchPlayerState() {
        playerPosture = isCrouch ? PlayerPosture.Crouch : PlayerPosture.Stand;

        if (moveInput.magnitude == 0) {
            locomotionState = LocomotionState.Idle;
        }
        else if (!isRunning) {
            locomotionState = LocomotionState.Walk;
        }
        else {
            locomotionState = LocomotionState.Run;
        }

        aimState = isAiming ? AimState.Aim : AimState.Normal;
    }

    void CalculateInputDirection() {
        Vector3 camForwardprojection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        playerMovement = camForwardprojection * moveInput.y + cameraTransform.right * moveInput.x;
        playerMovement = playerTransform.InverseTransformVector(playerMovement);
    }

    void SetupAnimator() {
        float moveSpeed = 0;
        const float DAMPTIME = 0.1f;

        animator.SetFloat(postureHash, (int)playerPosture, DAMPTIME, Time.deltaTime);
        if (playerPosture == PlayerPosture.Stand) {
            moveSpeed = locomotionState switch {
                LocomotionState.Idle => 0,
                LocomotionState.Walk => playerMovement.magnitude * walkSpeed,
                LocomotionState.Run => playerMovement.magnitude * runSpeed,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else if (playerPosture == PlayerPosture.Crouch) {
            moveSpeed = locomotionState switch {
                LocomotionState.Idle => 0,
                _ => playerMovement.magnitude * crouchSpeed
            };
        }

        animator.SetFloat(moveSpeedHash, moveSpeed, DAMPTIME, Time.deltaTime);

        if (aimState == AimState.Normal) {
            float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
            animator.SetFloat(turnSpeedHash, rad, DAMPTIME, Time.deltaTime);
            playerTransform.Rotate(0, rad * 200 * Time.deltaTime, 0);
        }
    }
}