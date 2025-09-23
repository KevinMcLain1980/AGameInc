using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private PlayerStat stat;
    [SerializeField] private Image fillImage;
    public float Normalized => Mathf.InverseLerp(minValue, maxValue, currentValue);

    private void Update()
    {
        fillImage.fillAmount = stat.Normalized;
    }
}
