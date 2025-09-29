using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingBarManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject startMenu;       // Assign Start Menu container
    public GameObject titleBanner;     // Assign Title Banner object
    public GameObject loadingScreen;   // Assign Loading Screen panel
    public GameObject backButton;      // Assign Back Button object
    public Slider loadingBar;          // Assign Loading Bar slider

    [Header("Timing")]
    public float fillSpeed = 0.5f;     // Speed of bar fill
    public float postFillDelay = 5f;   // Delay after bar reaches 100%

    public void LoadSceneAsync(string sceneName)
    {
        // Hide Start Menu and Title Banner
        if (startMenu != null) startMenu.SetActive(false);
        if (titleBanner != null) startMenu.SetActive(false);

        // Hide Back Button if active
        if (backButton != null && backButton.activeInHierarchy)
            backButton.SetActive(false);

        // Show Loading Screen
        loadingScreen.SetActive(true);

        // Begin loading
        StartCoroutine(LoadAsync(sceneName));
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float targetProgress = 0f;

        // Fill to 90% based on actual progress
        while (operation.progress < 0.9f)
        {
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, targetProgress, Time.deltaTime * fillSpeed);
            yield return null;
        }

        // Smoothly fill to 100%
        while (loadingBar.value < 1f)
        {
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, 1f, Time.deltaTime * fillSpeed);
            yield return null;
        }

        // Wait after bar is full
        yield return new WaitForSeconds(postFillDelay);

        // Activate the scene
        operation.allowSceneActivation = true;
    }
}
