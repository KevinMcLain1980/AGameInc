using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float fastRunSpeed = 12f;
    public float rotationSpeed = 120f;

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

    private float currentStamina;
    private bool isBreathing = false;
    private float breathingTimer = 0f;
    private bool justStoppedFastRunning = false;

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
    }

    void HandleMovement()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        bool isHoldingUp = vertical > 0;
        bool isHoldingDown = vertical < 0;
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift);

        float speed = 0f;

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
            animator.SetBool("IsRunningBackwards", true); // ← trigger backward animation
        }
        else
        {
            speed = 0f;
            animator.SetBool(isRunningFastParam, false);
            animator.SetBool(isRunningParam, false);
            animator.SetBool("IsRunningBackwards", false); // ← return to Idle
        }



        Vector3 move = transform.forward * vertical * speed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        animator.SetFloat(speedParam, Mathf.Abs(vertical * speed));

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
            animator.SetFloat("IdleSpeed", 0.5f); // Slower breathing
        }
        else
        {
            animator.SetFloat("IdleSpeed", 1f); // Reset to normal
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
