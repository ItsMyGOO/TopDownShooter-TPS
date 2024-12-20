using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class TopDownController : MonoBehaviour {
    static readonly int _verticalSpeed = Animator.StringToHash("Vertical Speed");
    static readonly int _rifle = Animator.StringToHash("Rifle");
    static readonly int _isAiming = Animator.StringToHash("isAiming");
    static readonly int _rightHandWeight = Animator.StringToHash("Right Hand Weight");
    static readonly int _leftHandWeight = Animator.StringToHash("Left Hand Weight");

    Animator animator;
    Transform playerTransform;
    CharacterController characterController;

    Vector2 playerInput;
    bool isRunning;

    Vector3 playerMovement;
    public float rotateSpeed = 1000;

    float currentSpeed;
    float targetSpeed;
    public float walkSpeed = 1.5f;
    public float runSpeed = 3.5f;
    public float speedLerp = 0.5f;

    bool armedRifle;
    bool isAiming;

    public GameObject rifleInHand;
    public GameObject rifleOnBack;

    public TwoBoneIKConstraint rightHandConstraint;
    public TwoBoneIKConstraint leftHandConstraint;

    void Start() {
        playerTransform = transform;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update() {
        RotatePlayer();
        MovePlayer();
        SetupAnimator();
        SetTwoHandsWeight();
    }

    void OnAnimatorMove() {
        characterController.SimpleMove(animator.velocity);
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
        isAiming = context.ReadValueAsButton();
        animator.SetBool(_isAiming, isAiming);
    }

    #endregion

    private void RotatePlayer() {
        if (playerInput.Equals(Vector2.zero))
            return;

        playerMovement.x = playerInput.x;
        playerMovement.z = playerInput.y;

        Quaternion rotation = Quaternion.LookRotation(playerMovement, Vector3.up);
        playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, rotation, rotateSpeed * Time.deltaTime);
    }

    void MovePlayer() {
        targetSpeed = isRunning ? runSpeed : walkSpeed;
        targetSpeed *= playerInput.magnitude;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedLerp);
    }

    void SetupAnimator() {
        animator.SetFloat(_verticalSpeed, currentSpeed);
    }

    public void PutGrabRifle(int param) {
        bool isGrab = param == 1;
        rifleInHand.SetActive(isGrab);
        rifleOnBack.SetActive(!isGrab);
    }

    void SetTwoHandsWeight() {
        rightHandConstraint.weight = animator.GetFloat(_rightHandWeight);
        leftHandConstraint.weight = animator.GetFloat(_leftHandWeight);
    }
}