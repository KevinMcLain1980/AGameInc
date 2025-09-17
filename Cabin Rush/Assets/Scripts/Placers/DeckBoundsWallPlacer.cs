using UnityEngine;

public class DeckBoundsWallPlacer : MonoBehaviour
{
    [Header("Short Wall Prefab")]
    public GameObject shortWallPrefab;

    [Header("Deck Settings")]
    public float deckSize = 75f;
    public int wallsPerSide = 11;
    public float wallSpacing = 7f;
    public float wallHeight = 10f;

    private float halfDeck;

    void Start()
    {
        if (shortWallPrefab == null)
        {
            Debug.LogError("Short Wall Prefab not assigned.");
            return;
        }

        halfDeck = deckSize / 2f;

        PlaceBottomEdge();
        PlaceTopEdge();
        PlaceLeftEdge();
        PlaceRightEdge();
    }

    void PlaceBottomEdge()
    {
        float z = -halfDeck;
        for (int i = 0; i < wallsPerSide; i++)
        {
            float x = -halfDeck + i * wallSpacing;
            Vector3 pos = new Vector3(x, wallHeight / 2f, z);
            Instantiate(shortWallPrefab, pos, Quaternion.identity, transform);
        }
    }

    void PlaceTopEdge()
    {
        float z = halfDeck;
        for (int i = 0; i < wallsPerSide; i++)
        {
            float x = -halfDeck + i * wallSpacing;
            Vector3 pos = new Vector3(x, wallHeight / 2f, z);
            Instantiate(shortWallPrefab, pos, Quaternion.identity, transform);
        }
    }

    void PlaceLeftEdge()
    {
        float x = -halfDeck;
        for (int i = 0; i < wallsPerSide; i++)
        {
            float z = -halfDeck + i * wallSpacing;
            Vector3 pos = new Vector3(x, wallHeight / 2f, z);
            Quaternion rot = Quaternion.Euler(0, 90, 0);
            Instantiate(shortWallPrefab, pos, rot, transform);
        }
    }

    void PlaceRightEdge()
    {
        float x = halfDeck;
        for (int i = 0; i < wallsPerSide; i++)
        {
            float z = -halfDeck + i * wallSpacing;
            Vector3 pos = new Vector3(x, wallHeight / 2f, z);
            Quaternion rot = Quaternion.Euler(0, 90, 0);
            Instantiate(shortWallPrefab, pos, rot, transform);
        }
    }
}
