using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(PlayerStateManager))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;
    public float slideForce = 10f;
    public float slideDuration = 1f;
    public float climbSpeed = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundCheckRadius = 0.4f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;

    [Header("Climb Settings")]
    public float climbRayDistance = 1f;
    public LayerMask climbableMask;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody rb;
    private CapsuleCollider col;
    private PlayerStateManager stateManager;

    private float moveSpeed;
    private bool isGrounded;
    private bool isSliding;
    private bool isClimbing;
    private bool isCrouching;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        stateManager = GetComponent<PlayerStateManager>();
        moveSpeed = walkSpeed;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovementInput();
        HandleJump();
        HandleCrouch();
        HandleSlide();
        HandleClimb();
        UpdateAnimator();
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

    void HandleMovementInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && !isSliding)
            moveSpeed = sprintSpeed;
        else if (isCrouching)
            moveSpeed = crouchSpeed;
        else
            moveSpeed = walkSpeed;

        if (!isSliding && !isClimbing)
        {
            rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
        }

        if (move.magnitude > 0 && isGrounded)
        {
            if (isCrouching)
                stateManager.SetState(MovementState.Crouching);
            else if (moveSpeed == sprintSpeed)
                stateManager.SetState(MovementState.Sprinting);
            else
                stateManager.SetState(MovementState.Walking);
        }
        else if (isGrounded)
        {
            stateManager.SetState(MovementState.Idle);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            stateManager.SetState(MovementState.Jumping);
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            col.height = crouchHeight;
            isCrouching = true;
            stateManager.SetState(MovementState.Crouching);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            col.height = standHeight;
            isCrouching = false;
            stateManager.SetState(MovementState.Walking);
        }
    }

    void HandleSlide()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && isGrounded && !isSliding)
        {
            StartCoroutine(Slide());
        }
    }

    IEnumerator Slide()
    {
        isSliding = true;
        stateManager.SetState(MovementState.Sliding);
        rb.AddForce(transform.forward * slideForce, ForceMode.Impulse);
        yield return new WaitForSeconds(slideDuration);
        isSliding = false;
        stateManager.SetState(MovementState.Walking);
    }

    void HandleClimb()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, climbRayDistance, climbableMask))
        {
            if (Input.GetKey(KeyCode.E))
            {
                isClimbing = true;
                rb.useGravity = false;
                rb.linearVelocity = new Vector3(0, climbSpeed, 0);
                stateManager.SetState(MovementState.Climbing);
            }
            else
            {
                isClimbing = false;
                rb.useGravity = true;
            }
        }
        else
        {
            isClimbing = false;
            rb.useGravity = true;
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetInteger("MovementState", (int)stateManager.currentState);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
