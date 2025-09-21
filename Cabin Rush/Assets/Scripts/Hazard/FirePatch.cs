using UnityEngine;

public class FirePatch : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 10f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateManager state = other.GetComponent<PlayerStateManager>();
            if (state != null)
            {
                state.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}
