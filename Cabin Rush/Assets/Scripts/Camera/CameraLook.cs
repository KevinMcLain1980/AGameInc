using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private Transform playerBody;

    private InputAction lookAction;
    private Vector2 lookInput;
    private float xRotation = 0f;

    private void Awake()
    {
        if (playerBody == null)
        {
            Debug.LogError("CameraLook: Player body reference not assigned.");
        }
    }

    private void OnEnable()
    {
        lookAction = new InputAction("Look", binding: "<Mouse>/delta");
        lookAction.Enable();
        lookAction.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        if (lookAction != null)
        {
            lookAction.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
            lookAction.Disable();
        }
    }

    private void Update()
    {
        if (playerBody == null) return;

        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
