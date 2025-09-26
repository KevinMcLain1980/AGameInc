using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStateManager playerStateManager;
    public bool IsDead => IsDead;

    private void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            playerStateManager.TakeDamage(10f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (playerStateManager.IsDead) return;

        // Reduce health
        PlayerStat currentHealth = GetComponent<PlayerStat>();
        if (currentHealth != null)
        {
            currentHealth.ModifyStat(-damageAmount);

            if (currentHealth.CurrentValue <= 0)
            {
                playerStateManager.TriggerDeath();
            }
        }
        else
        {
            Debug.LogWarning("HealthStat component not found on PlayerStateManager.");
        }
    }
}
