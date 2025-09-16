using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Bob Settings")]
    public float baseHeight = 1.64f;

    [Header("Forward Walk")]
    public float forwardBobFrequency = 1.5f;
    public float forwardBobAmplitude = 0.05f;

    [Header("Backward Walk")]
    public float backwardBobFrequency = 1.2f;
    public float backwardBobAmplitude = 0.025f;

    [Header("Running")]
    public float runBobFrequency = 2.5f;
    public float runBobAmplitude = 0.08f;

    private float bobTimer = 0f;

    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(verticalInput) > 0.01f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && verticalInput > 0;

        if (isMoving)
        {
            float frequency;
            float amplitude;

            if (isRunning)
            {
                frequency = runBobFrequency;
                amplitude = runBobAmplitude;
            }
            else if (verticalInput > 0)
            {
                frequency = forwardBobFrequency;
                amplitude = forwardBobAmplitude;
            }
            else
            {
                frequency = backwardBobFrequency;
                amplitude = backwardBobAmplitude;
            }

            bobTimer += Time.deltaTime * frequency;
            float bobOffset = Mathf.Sin(bobTimer) * amplitude;
            cameraTransform.localPosition = new Vector3(0, baseHeight + bobOffset, 0);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = new Vector3(0, baseHeight, 0);
        }
    }
}
