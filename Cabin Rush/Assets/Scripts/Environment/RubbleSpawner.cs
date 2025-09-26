using UnityEngine;
using System.Collections.Generic;

public class RubbleSpawner : MonoBehaviour
{
    [Header("Prefab and Placement Settings")]
    public GameObject rubblePrefab; // Assign Rubble_Big in Inspector
    public int count = 50;
    public Vector3 areaSize = new Vector3(60f, 0f, 60f); // Customize for Deck Scene
    public float yOffset = 0.2f; // Slight lift to avoid z-fighting
    public float raycastHeight = 10f;
    public float spacingRadius = 1.5f; // Minimum distance between rubble

    private List<Vector3> placedPositions = new List<Vector3>();

    void Start()
    {
        if (rubblePrefab == null)
        {
            Debug.LogError("Rubble prefab not assigned!");
            return;
        }

        int placed = 0;
        int attempts = 0;
        int maxAttempts = count * 20;

        while (placed < count && attempts < maxAttempts)
        {
            Vector3 localPos = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Vector3 worldPos = transform.position + localPos;
            Vector3 rayOrigin = worldPos + Vector3.up * raycastHeight;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f))
            {
                Vector3 spawnPos = hit.point + Vector3.up * yOffset;

                if (IsFarEnough(spawnPos))
                {
                    Instantiate(rubblePrefab, spawnPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
                    placedPositions.Add(spawnPos);
                    placed++;
                }
            }

            attempts++;
        }

        Debug.Log($"Placed {placed} rubble objects after {attempts} attempts.");
    }

    bool IsFarEnough(Vector3 newPos)
    {
        foreach (Vector3 pos in placedPositions)
        {
            if (Vector3.Distance(pos, newPos) < spacingRadius)
                return false;
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, yOffset, 0), areaSize);
    }
}
