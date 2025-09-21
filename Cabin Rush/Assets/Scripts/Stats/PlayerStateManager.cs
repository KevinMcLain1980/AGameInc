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

    private bool isDead = false;

    public void TakeDamage(float amount)
    {
        if (health == null || isDead) return;

        health.Modify(-damageAmount);
        statUI?.UpdateUI();
        animator.SetTrigger("TakeDamage");
        cameraShake?.TriggerShake();
        screenFlash?.TriggerFlash();
        damageAudio?.Play();

        if (health.CurrentValue <= 0)
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        isDead = true;
        animator.SetTrigger("DeathTrigger");
    }
}
