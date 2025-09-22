using UnityEngine;
using UnityEngine.UI;

public class OnTriggerEnterWater : MonoBehaviour
{
    public Image waterOverlay;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered water.");

            // Optional: activate overlay or trigger drowning logic
            DrownPlayer drown = other.GetComponent<DrownPlayer>();
            if (drown != null)
            {
                drown.SetWaterOverlay(true);
            }
        }
    }
}
