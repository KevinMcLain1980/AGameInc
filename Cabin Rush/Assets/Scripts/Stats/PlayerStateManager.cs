using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [Header("Animation & Camera")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private CameraSwitcher cameraSwitcher;

    private bool isDead = false;
    public bool IsDead => isDead;

    // Call this method when the player dies
    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        // Play death animation
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Death");
        }
        else
        {
            Debug.LogWarning("Player Animator not assigned in PlayerStateManager.");
        }

        // Enable camera switcher
        if (cameraSwitcher != null)
        {
            cameraSwitcher.enabled = true;
            cameraSwitcher.ActivateCinematic();
        }
        else
        {
            Debug.LogWarning("CameraSwitcher not assigned in PlayerStateManager.");
        }

        // Optional: disable movement, trigger UI, etc.
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        HealthStat healthStat = GetComponent<HealthStat>();
        if (healthStat != null)
        {
            healthStat.ModifyStat(-damageAmount);

            if (healthStat.CurrentValue <= 0)
            {
                TriggerDeath();
            }
        }
        else
        {
            Debug.LogWarning("HealthStat component not found on PlayerStateManager.");
        }
    }

    // Optional: Reset state if needed (e.g., on respawn)
    public void ResetState()
    {
        isDead = false;

        if (cameraSwitcher != null)
        {
            cameraSwitcher.enabled = false;
        }

        // Reset animator if needed
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Death");
        }
    }
}
