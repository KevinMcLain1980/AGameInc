using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StayUpright : MonoBehaviour
{
    [Header("Rotation Correction")]
    [Tooltip("How quickly the character returns to upright")]
    public float uprightSpeed = 10f;

    [Tooltip("Enable automatic upright correction")]
    public bool enableCorrection = true;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure Rigidbody constraints are set
        rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (!enableCorrection) return;

        // Preserve Y rotation (horizontal turning)
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, 0f);

        // Smoothly rotate back to upright
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, uprightSpeed * Time.fixedDeltaTime);
    }
}
