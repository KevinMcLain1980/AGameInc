using UnityEngine;

public class DrownPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Replace with your death logic
           
            // e.g., other.GetComponent<PlayerHealth>().Die();
        }
    }
}


