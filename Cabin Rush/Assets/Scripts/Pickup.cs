using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] SpeedPickup pickUp;
    [SerializeField] HealthPickup healthPickup;
    enum PickupType { Health, Speed};

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();

        if(pickup != null)
        {
            pickup.getSpeedProp(pickUp);
            Destroy(gameObject);
        }
    }

    private void HealthOnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();

        if (pickup != null)
        {
            pickup.getHealthProp(healthPickup);
            Destroy(gameObject);
        }
    }
}
