using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private PlayerStat stat;
    [SerializeField] private Image fillImage;

    private void Update()
    {
        fillImage.fillAmount = stat.Normalized;
        Debug.Log($"{stat.name} currentValue at start: {stat.currentValue}");
    }
}
