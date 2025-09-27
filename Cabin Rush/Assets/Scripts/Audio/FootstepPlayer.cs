using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float speedThreshold = 0.1f;

    private float stepTimer;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if (controller != null && controller.velocity.magnitude > speedThreshold)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                footstepSource.Play();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
}
