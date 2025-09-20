using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Vector3 originalPosition;
    private float shakeTimer;
    private bool shakeHorizontal;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            float offset = Random.Range(-shakeMagnitude, shakeMagnitude);

            Vector3 shakeOffset = shakeHorizontal
                ? new Vector3(offset, 0f, 0f)   // horizontal shake
                : new Vector3(0f, offset, 0f); // vertical shake

            transform.localPosition = originalPosition + shakeOffset;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }

    public void TriggerShake()
    {
        shakeTimer = shakeDuration;
        shakeHorizontal = Random.value > 0.5f; // randomly choose direction
    }
}
