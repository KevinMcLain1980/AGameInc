using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private PlayerStat health;
    [SerializeField] private PlayerStatUI statUI;
    [SerializeField] private float damageAmount = 10f;

    public void TakeDamage()
    {
        if (health != null)
        {
            health.Modify(-damageAmount);
        }

        if (statUI != null)
        {
            statUI.UpdateUI();
            Debug.Log("UI updated");
        }
    }
}
