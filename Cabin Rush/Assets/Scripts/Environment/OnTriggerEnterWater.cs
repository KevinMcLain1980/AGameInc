using UnityEngine;
using UnityEngine.UI;

public class OnTriggerEnterWater : MonoBehaviour
{
    public Image waterOverlay; // Assign your UI overlay in the inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered water.");

            if (waterOverlay != null)
                waterOverlay.enabled = true;

            DrownPlayer drown = other.GetComponent<DrownPlayer>();
          //  if (drown != null)
           //     drown.SetWaterOverlay(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited water.");

            if (waterOverlay != null)
                waterOverlay.enabled = false;

            DrownPlayer drown = other.GetComponent<DrownPlayer>();
          //  if (drown != null)
           //     drown.SetWaterOverlay(false);
        }
    }
}
