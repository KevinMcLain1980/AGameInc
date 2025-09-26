using UnityEngine;

public class WaterThreatFollower : MonoBehaviour
{
    [Header("Expansion Settings")]
    [SerializeField] private float horizontalFollowSpeed = 5f;
    [SerializeField] private float verticalExpandSpeed = 1f;
    [SerializeField] private float maxVerticalScale = 25f;

    [Header("References")]
    [SerializeField] private Transform player;

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Follow player horizontally
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, horizontalFollowSpeed * Time.deltaTime);

        // Expand vertically
        float newY = Mathf.Min(transform.localScale.y + verticalExpandSpeed * Time.deltaTime, maxVerticalScale);
        transform.localScale = new Vector3(transform.localScale.x, newY, transform.localScale.z);
    }
}
