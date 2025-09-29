using UnityEngine;

public class DrownPlayer : MonoBehaviour
{
    PlayerController player;
    [SerializeField] int Dmgamount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.takeOxygen(Dmgamount);
        }
    }
}
