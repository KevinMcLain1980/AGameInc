using UnityEngine;
public class WaterRise : MonoBehaviour
{
   [SerializeField] private float RiseSpeed = 1f;
   [SerializeField] private float MaxHeight = 10f;

    private PlayerMovementLogic InWater;

    private void Update()
    {
        if (transform.position.y < MaxHeight)
        {
            transform.position += Vector3.up * RiseSpeed * Time.deltaTime;
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InWater.moveSpeed -= Time.deltaTime;
        }
    }
}