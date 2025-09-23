using UnityEngine;
using UnityEngine.SceneManagement;

public class DiningRoomExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Cabin");
        }
    }
}
