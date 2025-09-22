using UnityEngine;
using UnityEngine.UI;

public class DrownPlayer : MonoBehaviour
{
    [Header("Player Oxygen Stat")]
    [SerializeField] private PlayerStat oxygen;

    [Header("Drain & Regen Settings")]
    [SerializeField] private float drainRate = 5f;
    [SerializeField] private float regenRate = 10f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("UI Overlay")]
    [SerializeField] private Image waterOverlay;

    private bool isDrowning = false;
    private bool isInWater = false;

    void Start()
    {
        if (oxygen != null)
            oxygen.ResetStat();
    }

    void Update()
    {
        if (oxygen == null || isDrowning) return;

        if (isInWater)
        {
            oxygen.Current -= drainRate * Time.deltaTime;

            if (oxygen.Current <= 0f)
            {
                oxygen.Current = 0f;
                isDrowning = true;
                animator.SetTrigger("DrownDeath");
            }
        }
        else
        {
            oxygen.Current += regenRate * Time.deltaTime;
            if (oxygen.Current > oxygen.Max)
                oxygen.Current = oxygen.Max;
        }
    }


    public void SetWaterOverlay(bool isActive)
    {
        Debug.Log("SetWaterOverlay called: " + isActive);
        if (waterOverlay != null)
            waterOverlay.enabled = isActive;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wat"))
        {
            isInWater = true;
            SetWaterOverlay(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("wat"))
        {
            isInWater = false;
            SetWaterOverlay(false);
        }
    }
}
