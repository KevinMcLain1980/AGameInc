using UnityEngine;

public class ThreatExpanderScript : MonoBehaviour
{
    [Header("Water Expansion Settings")]
    public Transform wat; // Assign your Water object here
    public float scaleRate = 0.1f; // Units per second

    void Start()
    {
        Debug.Log("Current Z Scale: " + wat.localScale.z);

        if (wat == null)
        {
            Debug.LogWarning("Water Transform 'wat' not assigned. Defaulting to self.");
            wat = transform;
        }
    }

    void Update()
    {
        // Continuously expand wat's scale on y axis
        wat.localScale += new Vector3(scaleRate * Time.deltaTime, 0f, 0f);
    }
}
