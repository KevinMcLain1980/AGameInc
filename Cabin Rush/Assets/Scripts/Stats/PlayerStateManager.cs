using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [Header("Animation & Camera")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private CameraSwitcher cameraSwitcher;

    [Header("Stat Reference")]
    [SerializeField] private PlayerStat healthStat;

    private bool isDead = false;
    public bool IsDead => isDead;

    /// <summary>
    /// Call this method to apply damage to the player.
    /// </summary>
    public void TakeDamage(float damageAmount)
    {
        if (isDead || healthStat == null) return;

        healthStat.ModifyStat(-damageAmount);

        if (healthStat.IsDepleted())
        {
            TriggerDeath();
        }
    }

    /// <summary>
    /// Triggers the death sequence: animation, camera switch, and disables movement.
    /// </summary>
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

        // Enable cinematic camera
        if (cameraSwitcher != null)
        {
            cameraSwitcher.enabled = true;
            cameraSwitcher.ActivateCinematic();
        }
        else
        {
            Debug.LogWarning("CameraSwitcher not assigned in PlayerStateManager.");
        }

        // Optional: disable movement, trigger death UI, etc.
    }

    /// <summary>
    /// Resets the player state (e.g., on respawn).
    /// </summary>
    public void ResetState()
    {
        isDead = false;

        if (cameraSwitcher != null)
        {
            cameraSwitcher.enabled = false;
        }

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Death");
        }

        if (healthStat != null)
        {
            healthStat.ResetStat();
        }
    }
}
