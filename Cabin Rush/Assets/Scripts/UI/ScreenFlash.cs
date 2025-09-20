using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float maxAlpha = 0.5f;

    private float flashTimer;

    private void Awake()
    {
        SetAlpha(0f);
    }

    private void Update()
    {
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0f, maxAlpha, flashTimer / flashDuration);
            SetAlpha(alpha);
        }
        else
        {
            SetAlpha(0f);
        }
    }

    public void TriggerFlash()
    {
        flashTimer = flashDuration;
        SetAlpha(maxAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (flashImage != null)
        {
            Color c = flashImage.color;
            c.a = alpha;
            flashImage.color = c;
        }
    }
}
