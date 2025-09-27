using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float strafeSpeed = 3f; // units per second

    private InputAction lookAction;
    private Vector2 lookInput;

    private void Awake()
    {
        if (playerBody == null)
        {
            Debug.LogError("CameraLook: Player body reference not assigned.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        // Mouse-based horizontal rotation
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        playerBody.Rotate(Vector3.up * mouseX);

        // Shift + Arrow Key Strafing
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            Vector3 strafeDirection = Vector3.zero;

            if (Keyboard.current.leftArrowKey.isPressed)
            {
                strafeDirection = -playerBody.right;
            }
            else if (Keyboard.current.rightArrowKey.isPressed)
            {
                strafeDirection = playerBody.right;
            }

            playerBody.position += strafeDirection * strafeSpeed * Time.deltaTime;
        }
    }
}
