using UnityEngine;
using UnityEngine.InputSystem;

public class TPSController : MonoBehaviour
{
    Transform playerTransform;
    Animator animator;

    public enum PlayerPosture
    {
        Crouch = 0,
        Stand,
        MidAir
    }
    public PlayerPosture playerPosture;

    public enum LocomotionState
    {
        Idle,
        Walk,
        Run
    }
    public LocomotionState locomotionState = LocomotionState.Idle;

    public enum AimState
    {
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

    private void Start()
    {
        playerTransform = transform;
        animator = GetComponent<Animator>();

        postureHash = Animator.StringToHash("posture");
        moveSpeedHash = Animator.StringToHash("moveSpeed");
        turnSpeedHash = Animator.StringToHash(" turnSpeed");
    }

    #region 玩家输入
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void GetRunInput(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }
    public void GetCrouchInput(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }
    public void GetAimInput(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }
    #endregion

    void SwitchPlayerState()
    {
        playerPosture = isCrouch ? PlayerPosture.Crouch : PlayerPosture.Stand;

        if (moveInput.magnitude == 0)
        {
            locomotionState = LocomotionState.Idle;
        }
        else if (!isRunning)
        {
            locomotionState = LocomotionState.Walk;
        }
        else
        {
            locomotionState = LocomotionState.Run;
        }

        aimState = isAiming ? AimState.Aim : AimState.Normal;
    }
}
