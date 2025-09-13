using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float fastRunSpeed = 12f;
    public float rotationSpeed = 120f;

    [Header("Jump Settings")]
    public float jumpForce = 28f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRecoveryRate = 10f;
    public float breathingDuration = 5f;

    [Header("Animation Parameters")]
    public string speedParam = "Speed";
    public string isRunningParam = "IsRunning";
    public string isRunningFastParam = "IsRunningFast";
    public string isBreathingParam = "IsBreathing";
    public string diveRollParam = "DiveRoll";
    public string slideTriggerParam = "SlideTrigger";

    private float currentStamina;
    private bool isBreathing = false;
    private float breathingTimer = 0f;
    private bool justStoppedFastRunning = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isCrouchWalking = false;
    public bool isSliding = false;

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;

        animator.SetBool(isRunningParam, false);
        animator.SetBool(isRunningFastParam, false);
        animator.SetBool(isBreathingParam, false);
        animator.SetFloat(speedParam, 0f);
        animator.SetFloat("IdleSpeed", 1f);
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleStamina();
        HandleDiveRoll();
        HandleJump();
        HandleCrouchToggle();
        HandleSlide();
    }

    void HandleMovement()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        vertical = Mathf.Abs(vertical) < 0.01f ? 0f : vertical;

        bool isHoldingUp = vertical > 0;
        bool isHoldingDown = vertical < 0;
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift);

        float speed = 0f;

        // Determine movement speed and animation flags
        if (isCrouching)
        {
            if (isHoldingUp)
            {
                speed = walkSpeed * 0.33f; // Crouch walk speed
                isCrouchWalking = true;
            }
            else
            {
                speed = 0f;
                isCrouchWalking = false;
            }

            animator.SetBool(isRunningFastParam, false);
            animator.SetBool(isRunningParam, false);
            animator.SetBool("IsRunningBackwards", false);
        }
        else
        {
            if (isHoldingUp && isHoldingShift && currentStamina > 0)
            {
                speed = fastRunSpeed;
                animator.SetBool(isRunningFastParam, true);
                animator.SetBool(isRunningParam, true);
            }
            else if (isHoldingUp && !isHoldingShift)
            {
                speed = runSpeed;
                animator.SetBool(isRunningFastParam, false);
                animator.SetBool(isRunningParam, true);
            }
            else if (isHoldingDown)
            {
                speed = walkSpeed * 0.5f;
                animator.SetBool(isRunningFastParam, false);
                animator.SetBool(isRunningParam, false);
                animator.SetBool("IsRunningBackwards", true);
            }
            else
            {
                speed = 0f;
                animator.SetBool(isRunningFastParam, false);
                animator.SetBool(isRunningParam, false);
                animator.SetBool("IsRunningBackwards", false);
            }

            isCrouchWalking = false;
        }

        // Apply movement
        if (!isJumping)
        {
            Vector3 move = transform.forward * vertical * speed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }

        // Update Animator parameters
        animator.SetFloat(speedParam, Mathf.Abs(vertical * speed));
        animator.SetBool("IsCrouchWalking", isCrouchWalking);

        // Optional: use crouch walking for stamina drain or stealth logic
        if (isCrouchWalking)
        {
            // Example: stamina drain while crouch walking
            currentStamina -= staminaDrainRate * Time.deltaTime * 0.5f;
        }

        // Handle fast run exit
        if ((!isHoldingUp || !isHoldingShift || currentStamina <= 0) && animator.GetBool(isRunningFastParam))
        {
            justStoppedFastRunning = true;
            animator.SetBool(isRunningFastParam, false);
            animator.SetBool(isRunningParam, false);
        }
    }

    void HandleRotation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            transform.Rotate(Vector3.up * horizontal * rotationSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool grounded = IsGrounded();

        Debug.Log($"JumpPressed: {jumpPressed}, IsGrounded: {grounded}");

        if (jumpPressed && grounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }
    }



    bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        return Physics.Raycast(ray, 1.2f, groundLayer);
    }


    void HandleStamina()
    {
        bool isFastRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0;

        if (isFastRunning && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if ((currentStamina <= 0 || justStoppedFastRunning) && !isBreathing)
        {
            isBreathing = true;
            breathingTimer = breathingDuration;
            animator.SetBool(isBreathingParam, true);
            animator.CrossFade("BreathingIdle", 0f);
            justStoppedFastRunning = false;
        }

        if (isBreathing)
        {
            animator.SetFloat("IdleSpeed", 0.5f);
        }
        else
        {
            animator.SetFloat("IdleSpeed", 1f);
        }
    }

    void HandleCrouchToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            animator.SetBool("IsCrouching", isCrouching);

            // Optional: adjust collider height
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            col.height = isCrouching ? 1f : 2f;
            col.center = new Vector3(0, isCrouching ? 0.5f : 1f, 0);
        }
    }

    void HandleSlide()
    {
        if (isSliding) return;

        bool slidePressed = Input.GetKeyDown(KeyCode.LeftAlt);
        bool isRunning = animator.GetBool(isRunningParam);

        if (slidePressed && isRunning)
        {
            isSliding = true;

            // Trigger slide animation
            animator.SetTrigger(slideTriggerParam);
            animator.SetFloat("SlideSpeed", 0.5f); // Optional: slow down animation

            // Apply forward slide movement
            Vector3 slideDirection = transform.forward * runSpeed * 2.5f;
            rb.linearVelocity = new Vector3(slideDirection.x, rb.linearVelocity.y, slideDirection.z);

            StartCoroutine(ResetSlide());
        }
    }

    IEnumerator ResetSlide()
    {
        yield return new WaitForSeconds(1.5f);
        isSliding = false;
        animator.ResetTrigger(slideTriggerParam);
        animator.Play("Idle"); 

        bool stillHoldingUp = Input.GetAxisRaw("Vertical") > 0;

        if (stillHoldingUp)
        {
            animator.SetBool(isRunningParam, true);
            animator.SetBool(isRunningFastParam, false);
            animator.SetFloat(speedParam, runSpeed);
        }
        else
        {
            animator.SetBool(isRunningParam, false);
            animator.SetBool(isRunningFastParam, false);
            animator.SetFloat(speedParam, 0f);
        }
    }

    void HandleDiveRoll()
    {
        bool isDiveRolling = Input.GetKeyDown(KeyCode.LeftControl) && Input.GetAxisRaw("Vertical") > 0;
        if (isDiveRolling)
        {
            animator.SetTrigger(diveRollParam);
        }
    }
}
