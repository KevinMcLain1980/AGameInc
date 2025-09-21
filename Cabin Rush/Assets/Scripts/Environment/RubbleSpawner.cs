using UnityEngine;

public class RubbleSpawner : MonoBehaviour
{
    [Header("Prefab and Placement Settings")]
    public GameObject rubblePrefab; // Assign Rubble_Big in Inspector
    public int count = 50;
    public Vector3 areaSize = new Vector3(60f, 0f, 60f); // Customize for Deck Scene
    public float yOffset = 0.5f; // Lift rubble slightly above ground

    [Header("Raycast Settings")]
    public bool useRaycast = true;
    public float raycastHeight = 10f;

    void Start()
    {
        if (rubblePrefab == null)
        {
            Debug.LogError("Rubble prefab not assigned!");
            return;
        }

        int placed = 0;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomXZ = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Vector3 spawnPos = transform.position + randomXZ;

            if (useRaycast)
            {
                Vector3 rayOrigin = spawnPos + Vector3.up * raycastHeight;
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f))
                {
                    spawnPos = hit.point + Vector3.up * yOffset;
                }
                else
                {
                    spawnPos += Vector3.up * yOffset;
                }
            }
            else
            {
                spawnPos += Vector3.up * yOffset;
            }

            Instantiate(rubblePrefab, spawnPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            placed++;
        }

        Debug.Log($"Placed {placed} rubble objects.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, yOffset, 0), areaSize);
    }
}
