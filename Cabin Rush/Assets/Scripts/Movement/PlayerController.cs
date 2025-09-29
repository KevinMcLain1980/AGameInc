using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IOxygen, IRun
{
    [SerializeField] private PlayerStateManager playerStateManager;
    [SerializeField] int Oxygen;
    [SerializeField] int Stamina;
    [SerializeField] int Hp;
    [SerializeField] int speed;
    [SerializeField] int speedMul;
    public bool IsDead => IsDead;
    bool IsRunning = false;

    int HpOrig;
    int OxygenOrig;
    int StaminaOrig;

    void Start()
    {
        HpOrig = Hp;
        OxygenOrig = Oxygen;
        StaminaOrig = Stamina;
    }

    void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            playerStateManager.TakeDamage(10f);
        }
        sprint();
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
        updatePlayerUI();
    }

    public void takeStamina(int amount)
    {
        if(Input.GetButtonDown("Sprint") && Stamina > 0)
        {
            IsRunning = true;
            Stamina -= amount;
            updatePlayerUI();
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            IsRunning = false;
        }
    }

    IEnumerator DrainStamina(IRun r)
    {
        IsRunning = true;
        r.takeStamina(2);
        yield return new WaitForSeconds(1);
        IsRunning = false;
    }

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = Hp / (float)HpOrig;
        GameManager.instance.playerOxygenBar.fillAmount = Oxygen / (float)OxygenOrig;
        if (IsRunning == true && Stamina > 0)
        {
            takeStamina(speedMul);
            GameManager.instance.playerStaminaBar.fillAmount = Stamina / (float)StaminaOrig;
            
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= speedMul;
            IsRunning = true;
            takeStamina(speedMul);
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= speedMul;
            IsRunning = false;
        }
    }
}
