using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementLogic : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float inputDeadzone = 0.2f;

    private CharacterController controller;
    private Animator animator;

    private InputAction moveAction;
    private InputAction rotateAction;
    private InputAction Crouch;
    private InputAction DiveRoll;
    private InputAction Slide;

    private Vector2 moveInput = Vector2.zero;
    private float rotationInput = 0f;
    private bool isCrouching = false;
    private bool isDiveRolling = false;
    private bool isSliding = false;

    public bool IsGrounded => controller != null && controller.isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (controller == null)
            Debug.LogError("PlayerMovementLogic: CharacterController not found.");

        if (animator == null)
            Debug.LogError("PlayerMovementLogic: Animator not found.");
    }

    private void OnEnable()
    {
        moveAction = new InputAction("Move");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick");
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
        moveAction.Enable();

        rotateAction = new InputAction("Rotate");
        rotateAction.AddBinding("<Keyboard>/leftArrow");
        rotateAction.AddBinding("<Keyboard>/rightArrow");
        rotateAction.performed += OnRotatePerformed;
        rotateAction.canceled += OnRotateCanceled;
        rotateAction.Enable();

        Crouch = new InputAction("Crouch", binding: "<Keyboard>/c");
        Crouch.performed += OnCrouchPerformed;
        Crouch.Enable();

        DiveRoll = new InputAction("DiveRoll", binding: "<Keyboard>/alt");
        DiveRoll.performed += OnDiveRollPerformed;
        DiveRoll.Enable();

        Slide = new InputAction("Slide", binding: "<Keyboard>/leftCtrl");
        Slide.performed += OnSlidePerformed;
        Slide.Enable();
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;
        moveAction.Disable();

        rotateAction.performed -= OnRotatePerformed;
        rotateAction.canceled -= OnRotateCanceled;
        rotateAction.Disable();

        Crouch.performed -= OnCrouchPerformed;
        Crouch.Disable();

        DiveRoll.performed -= OnDiveRollPerformed;
        DiveRoll.Disable();

        Slide.performed -= OnSlidePerformed;
        Slide.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 rawInput = ctx.ReadValue<Vector2>();
        moveInput = rawInput.magnitude < inputDeadzone ? Vector2.zero : rawInput;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnRotatePerformed(InputAction.CallbackContext ctx)
    {
        var key = ctx.control.name;
        rotationInput = key == "leftArrow" ? -1f : key == "rightArrow" ? 1f : 0f;
    }

    private void OnRotateCanceled(InputAction.CallbackContext ctx)
    {
        rotationInput = 0f;
    }

    private void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        isCrouching = !isCrouching;
        animator.SetBool("IsCrouching", isCrouching);

        controller.height = isCrouching ? 1f : 2f;
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        if (!isCrouching)
        {
            animator.SetBool("IsCrouchWalking", false);

            bool isMovingForward = moveInput.y > inputDeadzone;
            bool isMovingBackward = moveInput.y < -inputDeadzone;

            animator.SetBool("IsRunning", isMovingForward);
            animator.SetBool("IsWalkingBackwards", isMovingBackward);
            animator.speed = isMovingBackward ? 0.5f : 1f;
        }
        else
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalkingBackwards", false);
            animator.speed = 1f;
        }
    }

    private void OnDiveRollPerformed(InputAction.CallbackContext ctx)
    {
        if (isDiveRolling || isSliding) return;

        bool isForward = moveInput.y > inputDeadzone;
        bool isBackward = moveInput.y < -inputDeadzone;

        if (!isForward && !isBackward) return;

        StartCoroutine(PerformDiveRoll(isForward ? 1f : -1f));
    }

    private IEnumerator PerformDiveRoll(float direction)
    {
        isDiveRolling = true;
        animator.SetTrigger("DiveRoll");

        float rollDuration = 0.8f;
        float rollSpeed = moveSpeed * 0.5f;
        float elapsed = 0f;

        while (elapsed < rollDuration)
        {
            Vector3 rollDirection = transform.forward * direction;
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDiveRolling = false;

        bool forwardHeld = moveInput.y > inputDeadzone;
        bool backwardHeld = moveInput.y < -inputDeadzone;

        animator.SetBool("IsRunning", forwardHeld);
        animator.SetBool("IsWalkingBackwards", backwardHeld);
        animator.speed = backwardHeld ? 0.5f : 1f;
    }

    private void OnSlidePerformed(InputAction.CallbackContext ctx)
    {
        if (isSliding || isDiveRolling) return;

        // Read current input directly
        Vector2 currentInput = moveAction.ReadValue<Vector2>();
        bool forwardHeld = currentInput.y > inputDeadzone;

        // Confirm Animator is in Running state
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        bool isInRunningState = state.IsName("Running");

        if (forwardHeld && isInRunningState)
        {
            StartCoroutine(PerformSlide());
        }
    }

    private IEnumerator PerformSlide()
    {
        isSliding = true;

        // Interrupt Running animation
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalkingBackwards", false);
        animator.SetTrigger("Slide");

        float slideDuration = 0.6f; // Match animation length
        float slideSpeed = moveSpeed * 1.5f;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            Vector3 slideDirection = transform.forward;
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isSliding = false;

        // Check current input after slide
        Vector2 currentInput = moveAction.ReadValue<Vector2>();
        bool forwardHeld = currentInput.y > inputDeadzone;

        animator.SetBool("IsRunning", forwardHeld);
        animator.SetBool("IsWalkingBackwards", false);
        animator.speed = 1f;
    }
    private void Update()
    {
        if (isDiveRolling || isSliding)
            return;

        if (rotationInput != 0f)
        {
            transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
        }

        bool isMovingForward = moveInput.y > inputDeadzone;
        bool isMovingBackward = moveInput.y < -inputDeadzone;
        bool isMoving = moveInput != Vector2.zero;

        if (isCrouching)
        {
            bool isCrouchWalking = isMovingForward || isMovingBackward;
            animator.SetBool("IsCrouchWalking", isCrouchWalking);

            float crouchMoveSpeed = moveSpeed * 0.5f;
            Vector3 crouchDirection = transform.forward * moveInput.y;
            controller.Move(crouchDirection.normalized * crouchMoveSpeed * Time.deltaTime);
            return;
        }

        if (!isMoving)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalkingBackwards", false);
            animator.speed = 1f;
            return;
        }

        Vector3 moveDirection = transform.forward * moveInput.y;
        float currentSpeed = isMovingBackward ? moveSpeed * 0.5f : moveSpeed;

        controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);

        animator.SetBool("IsRunning", isMovingForward);
        animator.SetBool("IsWalkingBackwards", isMovingBackward);
        animator.speed = isMovingBackward ? 0.5f : 1f;
    }

    public void SetAnimatorSync(Animator animatorRef)
    {
        animator = animatorRef;
    }
}
