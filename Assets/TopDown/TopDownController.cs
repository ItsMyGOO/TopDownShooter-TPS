using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownController : MonoBehaviour {
    static readonly int _verticalSpeed = Animator.StringToHash("Vertical Speed");
    static readonly int _rifle = Animator.StringToHash("Rifle");
    static readonly int _isAiming = Animator.StringToHash("isAiming");

    Animator animator;
    Transform playerTransform;

    Vector2 playerInput;
    bool isRunning;

    Vector3 playerMovement;
    // public float rotateSpeed = 1000;

    float currentSpeed;
    float targetSpeed;
    public float walkSpeed = 1.5f;
    public float runSpeed = 3.5f;
    public float speedLerp = 0.5f;

    bool armedRifle;
    bool isAiming;

    void Start() {
        playerTransform = transform;
        animator = GetComponent<Animator>();
    }

    void Update() {
        RotatePlayer();
        MovePlayer();
        SetupAnimator();
    }

    #region MyRegion 玩家输入

    public void GetPlayerMoveInput(InputAction.CallbackContext context) {
        playerInput = context.ReadValue<Vector2>();
    }

    public void GetPlayerRunInput(InputAction.CallbackContext context) {
        isRunning = context.ReadValueAsButton();
    }

    public void GetArmedRifleInput(InputAction.CallbackContext context) {
        if (context.performed) {
            armedRifle = !armedRifle;
            animator.SetBool(_rifle, armedRifle);
        }
    }

    public void GetPlayerAimInput(InputAction.CallbackContext context) {
        // isAiming = context.ReadValueAsButton();
        if (context.performed) {
            isAiming = !isAiming;
        }

        animator.SetBool(_isAiming, isAiming);
    }

    #endregion

    private void RotatePlayer() {
        if (playerInput.Equals(Vector2.zero))
            return;

        playerMovement.x = playerInput.x;
        playerMovement.z = playerInput.y;

        Quaternion rotation = Quaternion.LookRotation(playerMovement, Vector3.up);
        // playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, rotation, rotateSpeed * Time.deltaTime);
        playerTransform.rotation = rotation;
    }

    void MovePlayer() {
        targetSpeed = isRunning ? runSpeed : walkSpeed;
        targetSpeed *= playerInput.magnitude;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedLerp);
    }

    void SetupAnimator() {
        animator.SetFloat(_verticalSpeed, currentSpeed);
    }
}