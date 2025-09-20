using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStateManager stateManager;

    private void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            stateManager.TakeDamage();
        }
    }
}
