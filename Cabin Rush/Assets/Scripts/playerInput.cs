using UnityEngine;
using UnityEngine.InputSystem;


public class playerInput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputActionAsset inputActions;
    
    
    
    private void OnEnable()
    {
        inputActions.Enable();
    }

    // Update is called once per frame
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
