using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private PlayerStat health;
    [SerializeField] private PlayerStatUI statUI;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private ScreenFlash screenFlash;

    [Header("Audio")]
    [SerializeField] private AudioSource damageAudio;

    private Animator animator;
    private CameraShake cameraShake;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    public void TakeDamage()
    {
        if (health != null)
        {
            damageAudio?.Play();
            health.Modify(-damageAmount);
            animator.SetTrigger("TakeDamage");
            cameraShake?.TriggerShake();
            screenFlash?.TriggerFlash();

        }

        if (statUI != null)
        {
            statUI.UpdateUI();
            Debug.Log("UI updated");
        }
    }
}
