using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderDiningHall: MonoBehaviour
{
    public void LoadGrandDiningHall()
    {
        SceneManager.LoadScene("Grand Dining Hall");
        Debug.Log("Attempting to load scene: Grand Dining Hall");
    }
}
