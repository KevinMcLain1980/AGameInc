using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private PlayerStat stat;
    [SerializeField] private Image fillImage;

    private void Update()
    {
        if (stat != null && fillImage != null)
        {
            fillImage.fillAmount = stat.Normalized;
        }
    }
}
