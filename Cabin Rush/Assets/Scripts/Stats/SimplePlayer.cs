using UnityEngine;
using UnityEngine.InputSystem;

public class SimplePlayer : MonoBehaviour
{
    [SerializeField] private PlayerStat health;
    [SerializeField] private PlayerStat oxygen;
    [SerializeField] private PlayerStat stamina;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.TestHealth.performed += ctx => health.ModifyStat(-10f);
        controls.Player.TestOxygen.performed += ctx => oxygen.ModifyStat(-5f);
        controls.Player.TestStamina.performed += ctx => stamina.ModifyStat(-15f);
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}
