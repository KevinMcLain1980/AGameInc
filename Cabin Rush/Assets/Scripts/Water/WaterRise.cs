using UnityEngine;
public class WaterRise : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 1f;
    [SerializeField] private float maxHeight = 10f;

    private void Update()
    {
        if (transform.position.y < maxHeight)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
    }
}
