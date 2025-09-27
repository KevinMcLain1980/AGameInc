using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            GameManager.instance.Win();
        }
    }
}
