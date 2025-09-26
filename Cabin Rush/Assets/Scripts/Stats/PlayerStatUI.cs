using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    [Header("Stat References")]
    [SerializeField] private PlayerStat healthStat;
    [SerializeField] private PlayerStat oxygenStat;
    [SerializeField] private PlayerStat staminaStat;

    [Header("UI Fill Images")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image oxygenFill;
    [SerializeField] private Image staminaFill;

    private void Update()
    {
        if (healthStat != null && healthFill != null)
            healthFill.fillAmount = healthStat.Normalized;

        if (oxygenStat != null && oxygenFill != null)
            oxygenFill.fillAmount = oxygenStat.Normalized;

        if (staminaStat != null && staminaFill != null)
            staminaFill.fillAmount = staminaStat.Normalized;
    }
    public void UpdateUI()
    {
        if (healthStat != null && healthFill != null)
            healthFill.fillAmount = healthStat.Normalized;

        if (oxygenStat != null && oxygenFill != null)
            oxygenFill.fillAmount = oxygenStat.Normalized;

        if (staminaStat != null && staminaFill != null)
            staminaFill.fillAmount = staminaStat.Normalized;
    }
}
