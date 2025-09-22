using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    [Header("UI Fill Images")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private Image oxygenBarFill;

    [Header("Player Stats")]
    [SerializeField] private PlayerStat health;
    [SerializeField] private PlayerStat stamina;
    [SerializeField] private PlayerStat oxygen;

    private void Awake()
    {
       
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        SyncBar(healthBarFill, health);
        SyncBar(staminaBarFill, stamina);
        SyncBar(oxygenBarFill, oxygen);
    }

    private void SyncBar(Image barFill, PlayerStat stat)
    {
        if (barFill == null || stat == null) return;

        float targetFill = Mathf.Clamp01(stat.Normalized);
        barFill.fillAmount = targetFill;

    }

}
