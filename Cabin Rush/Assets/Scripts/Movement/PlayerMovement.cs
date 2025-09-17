using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
<<<<<<< HEAD
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform climbAnchor;
=======



public class PlayerMovement : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] int oxygen;

>>>>>>> parent of 9033d64 (Oxy/HP depletion)

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float fastRunSpeed = 12f;
    public float rotationSpeed = 120f;
    private CharacterController characterController;

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
    private bool isDiveRolling = false;
    private bool diveRollOnCooldown = false;
    public string diveRollTriggerParam = "DiveRollTrigger";
    public float diveRollDuration = 1.2f;
    public float diveRollCooldown = 2f;

    [Header("Climbing Parameters")]
    private bool isNearObstacle;
    [SerializeField] private Transform obstacleSnapPoint;
    private bool isPlayingHangingClimb = false;


    private Animator animator;
    private Rigidbody rb;

    void Start()

    {
<<<<<<< HEAD
        characterController = GetComponent<CharacterController>();
=======
>>>>>>> parent of 9033d64 (Oxy/HP depletion)
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
        HandleDiveRoll();
        HandleMovement();

        if (isNearObstacle && Input.GetKeyDown(KeyCode.Space))
        {
            TriggerHangingClimb();
        }
        HandleJump();
        HandleCrouchToggle();
        HandleSlide();
    }

    private IEnumerator HandleObstacleClimb()
    {
        if (obstacleSnapPoint == null)
        {
            Debug.LogWarning("ObstacleSnapPoint not assigned.");
            yield break;
        }

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        // Play animation
        animator.Play("HangingClimb", 0, 0f); // Ensure this matches your Animator state name

        // Wait for animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Move player to snap point
        transform.position = obstacleSnapPoint.position;

        rb.useGravity = true;
        animator.Play("Idle", 0, 0f);
    }

    private void TriggerHangingClimb()
    {
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        isPlayingHangingClimb = true;

        animator.ResetTrigger("HangingClimbTrigger");
        animator.SetTrigger("HangingClimbTrigger");

        StartCoroutine(HandleHangingClimb());
    }

    void HandleMovement()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        vertical = Mathf.Abs(vertical) < 0.01f ? 0f : vertical;

        bool isHoldingUp = vertical > 0;
        bool isHoldingDown = vertical < 0;
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift);

        if (isPlayingHangingClimb) return;

        if (isCrouching && isHoldingDown)
        {
            isCrouching = false;
            animator.SetBool("IsCrouching", false);
            StartCoroutine(SmoothUncrouch());

            float walkspeed = walkSpeed * 0.5f;
            Vector3 move = -transform.forward * walkspeed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

            animator.SetBool("IsRunningBackwards", true);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsRunningFast", false);
            animator.SetBool("IsCrouchWalking", false);
            animator.SetFloat(speedParam, walkspeed);
            return;
        }

        if (!isHoldingUp && !isHoldingDown)
        {
            animator.SetBool("IsRunningBackwards", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsRunningFast", false);
            animator.SetBool("IsCrouchWalking", false);
            animator.SetFloat(speedParam, 0f);
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        float speed = 0f;

        if (isCrouching)
        {
            if (isHoldingUp)
            {
                speed = walkSpeed * 0.33f;
                isCrouchWalking = true;
            }
            else
            {
                speed = 0f;
                isCrouchWalking = false;
            }

            animator.SetBool("IsRunningBackwards", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsRunningFast", false);
            animator.SetBool("IsCrouchWalking", isCrouchWalking);
        }
        else
        {
            if (isHoldingUp && isHoldingShift && currentStamina > 0)
            {
                speed = fastRunSpeed;
                animator.SetBool("IsRunningFast", true);
                animator.SetBool("IsRunning", true);
            }
            else if (isHoldingUp && !isHoldingShift)
            {
                speed = runSpeed;
                animator.SetBool("IsRunningFast", false);
                animator.SetBool("IsRunning", true);
            }
            else if (isHoldingDown)
            {
                speed = walkSpeed * 0.5f;
                animator.SetBool("IsRunningBackwards", true);
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsRunningFast", false);
            }
            else
            {
                speed = 0f;
                animator.SetBool("IsRunningBackwards", false);
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsRunningFast", false);
            }

            animator.SetBool("IsCrouchWalking", false);
        }

        if (!isJumping)
        {
            Vector3 move = transform.forward * vertical * speed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }

        animator.SetFloat(speedParam, Mathf.Abs(vertical * speed));

        if (isCrouchWalking)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime * 0.5f;
        }

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
            breathingTimer -= Time.deltaTime;
            if (breathingTimer <= 0f)
            {
                isBreathing = false;
                animator.SetBool("isBreathingParam", false);
            }
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

        bool stillHoldingUp = Input.GetAxisRaw("Vertical") > 0;
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift);

        if (stillHoldingUp && isHoldingShift && currentStamina > 0)
        {
            animator.SetBool(isRunningParam, true);
            animator.SetBool(isRunningFastParam, true);
            animator.SetFloat(speedParam, fastRunSpeed);
        }
        else if (stillHoldingUp)
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
    IEnumerator SmoothUncrouch()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        float duration = 0.1f;
        float elapsed = 0f;

        float startHeight = col.height;
        float targetHeight = 2f;

        Vector3 startCenter = col.center;
        Vector3 targetCenter = new Vector3(0, 1f, 0);

        while (elapsed < duration)
        {
            col.height = Mathf.Lerp(startHeight, targetHeight, elapsed / duration);
            col.center = Vector3.Lerp(startCenter, targetCenter, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        col.height = targetHeight;
        col.center = targetCenter;

        // ✅ Snap player to ground after uncrouch
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f, groundLayer))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    void HandleDiveRoll()
    {
        // Prevent triggering during roll or cooldown
        if (isDiveRolling || diveRollOnCooldown) return;

        // Input detection
        bool divePressed = Input.GetKeyDown(KeyCode.X);
        bool upArrowHeld = Input.GetKey(KeyCode.UpArrow);
        bool isGrounded = IsGrounded();

        // Allow Dive Roll from Idle or Running, or when UpArrow + X are pressed together
        if (divePressed && isGrounded && (upArrowHeld || animator.GetFloat(speedParam) == 0f))
        {
            isDiveRolling = true;
            diveRollOnCooldown = true;

            // Trigger animation
            animator.ResetTrigger(diveRollTriggerParam);
            animator.SetTrigger(diveRollTriggerParam);

            // Optional: force immediate start
            animator.Play("DiveRoll", 0, 0f); // Ensure "DiveRoll" matches Animator state name

            // Apply forward movement burst
            Vector3 rollDirection = transform.forward * runSpeed * 2f;
            rb.linearVelocity = new Vector3(rollDirection.x, rb.linearVelocity.y, rollDirection.z);

            // Start recovery coroutine
            StartCoroutine(ResetDiveRoll());
        }
    }

    private IEnumerator HandleHangingClimb()
    {
        if (obstacleSnapPoint == null)
        {
            Debug.LogWarning("ObstacleSnapPoint not assigned.");
            yield break;
        }

        // Wait until the animation state is active
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("HangingClimb"));
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        // Slight upward and forward nudge
        Vector3 climbOffset = transform.forward * 0.5f + Vector3.up * 0.3f;
        transform.position += climbOffset;

        transform.position = obstacleSnapPoint.position + climbOffset;

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        isPlayingHangingClimb = false;

        animator.SetFloat(speedParam, 0f); // Let Animator return to Idle naturally
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isNearObstacle = true;
            Debug.Log("Near obstacle");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isNearObstacle = false;
        }
    }


    IEnumerator ResetDiveRoll()
    {
        yield return new WaitForSeconds(diveRollCooldown);
        isDiveRolling = false;
        diveRollOnCooldown = false;

        float vertical = Input.GetAxisRaw("Vertical");

        if (vertical > 0)
        {
            animator.SetBool(isRunningParam, true);
            animator.SetBool("IsRunningBackwards", false);
            animator.SetFloat(speedParam, runSpeed);
        }
        else if (vertical < 0)
        {
            animator.SetBool("IsRunningBackwards", true);
            animator.SetBool(isRunningParam, false);
            animator.SetFloat(speedParam, walkSpeed * 0.5f);
        }
        else
        {
            animator.SetBool(isRunningParam, false);
            animator.SetBool("IsRunningBackwards", false);
            animator.SetFloat(speedParam, 0f);
        }
    }

<<<<<<< HEAD
=======
    public void takeDamage(int amount)
    {
        if(HP <= 0)
        {
            // Already dead, ignore further damage
        }
    }
>>>>>>> parent of 9033d64 (Oxy/HP depletion)
}
