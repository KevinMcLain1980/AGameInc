using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]



public class PlayerMovement : MonoBehaviour, IDamage,IOxygen
{
    [SerializeField] int HP;
    [SerializeField] int oxygen;

    int HPOrig;
    int oxygenOrig;


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
    private bool isDiveRolling = false;
    private float diveRollCooldown = 2f;
    private float diveRollTimer = 0f;
    public string diveRollTriggerParam = "DiveRollTrigger";

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        HPOrig = HP;
        oxygenOrig = oxygen;
        updatePlayerUI();


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

        // 🔧 Optional stamina drain for crouch walking
        if (isCrouchWalking)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime * 0.5f;
        }

        // 🔧 Handle fast run exit
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
        if (isDiveRolling) return;

        bool divePressed = Input.GetKeyDown(KeyCode.X);
        bool isIdleOrRunning = animator.GetBool(isRunningParam) || animator.GetFloat(speedParam) == 0f;

        if (divePressed && isIdleOrRunning)
        {
            isDiveRolling = true;
            animator.SetTrigger(diveRollTriggerParam);

            // Apply forward roll movement
            Vector3 rollDirection = transform.forward * runSpeed * 2f;
            rb.linearVelocity = new Vector3(rollDirection.x, rb.linearVelocity.y, rollDirection.z);

            StartCoroutine(ResetDiveRoll());
        }
    }

    IEnumerator ResetDiveRoll()
    {
        // Wait for animation to finish
        yield return new WaitForSeconds(1.2f); // Match Dive Roll animation length

        // Wait for movement to settle
        yield return new WaitForSeconds(0.8f); // Optional: allow physics to stabilize

        isDiveRolling = false;

        // Transition based on input
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

    public void takeDamage(int amount)
    {
        oxygen -= amount;
        if (oxygen > 0)
        {
            // Player has oxygen, damage is ignored
            return;
        }
      
        HP -= amount;
        updatePlayerUI();

        if (HP <= 0)
        {
            // Player is dead — handle death logic here
        }
    }

    void updatePlayerUI()
    {
               GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        GameManager.instance.oxygenBar.fillAmount = (float)oxygen / oxygenOrig;
    }

    public void takeOxygen(int amount)
    {
        oxygen -= amount;
        if (oxygen < 0) oxygen = 0;

        updatePlayerUI();

        if (oxygen <= 0)
        {
            // Player can't breathe — handle suffocation logic here
        }
    }
}
