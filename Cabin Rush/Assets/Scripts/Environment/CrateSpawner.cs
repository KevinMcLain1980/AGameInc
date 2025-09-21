using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    [Header("Crate Settings")]
    public GameObject cratePrefab; // Assign your crate prefab in Inspector
    public int crateCount = 20;

    [Header("Spawn Area")]
    public Vector3 areaSize = new Vector3(40f, 0f, 20f); // Adjust to match deck size
    public float yOffset = 0.5f; // Lift crates slightly above surface

    [Header("Raycast Grounding")]
    public bool useRaycast = true;
    public float raycastHeight = 10f;

    void Start()
    {
        if (cratePrefab == null)
        {
            Debug.LogError("Crate prefab not assigned!");
            return;
        }

        for (int i = 0; i < crateCount; i++)
        {
            Vector3 localPos = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Vector3 worldPos = transform.position + localPos;

            if (useRaycast)
            {
                Vector3 rayOrigin = worldPos + Vector3.up * raycastHeight;
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f))
                {
                    worldPos = hit.point + Vector3.up * yOffset;
                }
                else
                {
                    worldPos += Vector3.up * yOffset;
                }
            }
            else
            {
                worldPos += Vector3.up * yOffset;
            }

            Instantiate(cratePrefab, worldPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }

        Debug.Log($"Spawned {crateCount} crates.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, yOffset, 0), areaSize);
    }
}
