using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform climbAnchor;
    [SerializeField] public Slider staminaSlider;
    [SerializeField] private Slider healthSlider;


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

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Oxygen Settings")]
    public float maxOxygen = 100f;
    public float currentOxygen = 100f;
    public float oxygenRecoveryRate = 10f;
    public float oxygenDrainRate = 20f;
    [SerializeField] private Image oxygenFillImage;


    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRecoveryRate = 10f;
    public float breathingDuration = 5f;

    [Header("Environment States")]
    public bool isUnderwater = false;

    [Header("Animation Parameters")]
    public string speedParam = "Speed";
    public string isRunningParam = "IsRunning";
    public string isRunningFastParam = "IsRunningFast";
    public string isBreathingParam = "IsBreathing";
    public string diveRollParam = "DiveRoll";
    public string slideTriggerParam = "SlideTrigger";
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
        if (staminaSlider == null)
        Debug.LogWarning("Stamina Slider not assigned!");

        characterController = GetComponent<CharacterController>();
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
        // Rotation Input
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            rotationInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            rotationInput = 1f;

        if (rotationInput != 0f)
        {
            transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
        }


        HandleOxygen();
        UpdateUIBars();
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

    void HandleOxygen()
    {
        if (isUnderwater)
        {
            currentOxygen -= oxygenDrainRate * Time.deltaTime;
        }
        else
        {
            currentOxygen += oxygenRecoveryRate * Time.deltaTime;
        }

        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
        GameManager.instance.UpdatePlayerOxygen(currentOxygen, maxOxygen);
    }



    void UpdateUIBars()
    {
        // Debug logs for live tracking
        Debug.Log($"Health: {currentHealth}/{maxHealth}");
        Debug.Log($"Stamina: {currentStamina}/{maxStamina}");
        Debug.Log($"Oxygen: {currentOxygen}/{maxOxygen}");

        // Clamp values to avoid overflow/underflow
        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        float staminaPercent = Mathf.Clamp01(currentStamina / maxStamina);
        float oxygenPercent = Mathf.Clamp01(currentOxygen / maxOxygen);

        // Update GameManager UI references
        if (GameManager.instance.playerHPBar != null)
            GameManager.instance.playerHPBar.value = healthPercent;

        if (GameManager.instance.playerStaminaBar != null)
            GameManager.instance.playerStaminaBar.value = staminaPercent;

        if (GameManager.instance.playerOxygenBar != null)
            GameManager.instance.playerOxygenBar.value = oxygenPercent;

        // Update local sliders (if used)
        if (healthSlider != null)
            healthSlider.value = healthPercent;

        if (staminaSlider != null)
            staminaSlider.value = staminaPercent;

        // Optional: if oxygen uses Image.fillAmount
        if (oxygenFillImage != null)
            oxygenFillImage.fillAmount = oxygenPercent;
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

        float speed = 0f;

        // Handle crouch walking
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
            // Handle RunningFast with stamina drain
            if (isHoldingUp && isHoldingShift && currentStamina > 0f)
            {
                speed = fastRunSpeed;
                currentStamina -= staminaDrainRate * Time.deltaTime;

                animator.SetBool("IsRunningFast", true);
                animator.SetBool("IsRunning", true);

                if (currentStamina <= 0f)
                {
                    currentStamina = 0f;
                    justStoppedFastRunning = true;

                    // Transition to Running
                    speed = runSpeed;
                    animator.SetBool("IsRunningFast", false);
                    animator.SetBool("IsRunning", true);
                }
            }
            else if (isHoldingUp)
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

        // Apply movement
        if (!isJumping)
        {
            Vector3 move = transform.forward * vertical * speed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }

        animator.SetFloat(speedParam, Mathf.Abs(vertical * speed));

        // Clamp stamina
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // Handle breathing state
        if ((!isHoldingUp || !isHoldingShift || currentStamina <= 0f) && animator.GetBool(isRunningFastParam))
        {
            justStoppedFastRunning = true;
            animator.SetBool(isRunningFastParam, false);
            animator.SetBool(isRunningParam, true); // Transition to regular running
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
        bool isTryingToRunFast = Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0;
        bool isActuallyRunningFast = animator.GetBool(isRunningFastParam);
        staminaSlider.value = currentStamina / maxStamina;

        // Drain stamina only while actively RunningFast
        if (isActuallyRunningFast && currentStamina > 0f)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                justStoppedFastRunning = true;

                // Transition to regular running
                animator.SetBool(isRunningFastParam, false);
                animator.SetBool(isRunningParam, true);
            }
        }
        // Recover stamina only when not trying to Run Fast
        else if (!isTryingToRunFast)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // Handle breathing animation when stamina hits zero
        if ((currentStamina <= 0f || justStoppedFastRunning) && !isBreathing)
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
                animator.SetBool(isBreathingParam, false);
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
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        GameManager.instance.UpdatePlayerHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Debug.Log("Player has died.");
            GameManager.instance.Loser();
            // Add death animation or disable movement here
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

}
