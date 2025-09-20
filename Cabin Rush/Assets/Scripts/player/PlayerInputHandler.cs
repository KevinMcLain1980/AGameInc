using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public float RotateInput { get; private set; }
    public bool JumpPressed => jumpPressed;
    public bool ClimbPressed => climbPressed;

    private PlayerControls controls;
    private bool jumpPressed;
    private bool climbPressed;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Rotate.performed += ctx => RotateInput = ctx.ReadValue<float>();
        controls.Player.Jump.performed += ctx => jumpPressed = true;
        controls.Player.Climb.performed += ctx => climbPressed = true;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void LateUpdate()
    {
        jumpPressed = false;
        climbPressed = false;
    }
}
