using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderStartMenu : MonoBehaviour
{
    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
        Debug.Log("Attempting to load scene: Start Menu");
    }
}
