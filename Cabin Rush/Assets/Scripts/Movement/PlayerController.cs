using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IOxygen, IRun
{
    [SerializeField] private PlayerStateManager playerStateManager;
    [SerializeField] int Oxygen;
    [SerializeField] int Stamina;
    public bool IsDead => IsDead;
    public bool IsRunning;

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

    public void takeOxygen(int amount)
    {
        Oxygen -= amount;
    }

    public void takeStamina(int amount)
    {
        if(Input.GetButtonDown("Sprint") && Stamina > 0)
        {
            Stamina -= amount;
        }
    }

    IEnumerator DrainStamina(IRun r)
    {
        IsRunning = true;
        r.takeStamina(2);
        yield return new WaitForSeconds(1);
        IsRunning = false;
    }
}
