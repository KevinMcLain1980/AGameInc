using UnityEngine;

public class PlayerJumpHandler : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animatorSync;
    private PlayerMovementLogic movementLogic;

    [SerializeField] private float jumpForce = 28f;

    public void SetAnimatorSync(Animator sync) => animatorSync = sync;
    public void SetMovementLogic(PlayerMovementLogic logic) => movementLogic = logic;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animatorSync == null)
            animatorSync = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("PlayerJumpHandler: Rigidbody is missing.");

        if (animatorSync == null)
            Debug.LogWarning("PlayerJumpHandler: Animator is missing. Jump and climb triggers may fail.");
    }

    public void Jump()
    {
        if (rb != null)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (animatorSync != null)
            animatorSync.SetTrigger("Jump");
    }

    public void TriggerClimb()
    {
        if (animatorSync != null)
        {
            animatorSync.SetTrigger("Climb");
            animatorSync.SetBool("IsClimbing", true);
        }
    }

    public void StopClimb()
    {
        if (animatorSync != null)
            animatorSync.SetBool("IsClimbing", false);
    }
}
