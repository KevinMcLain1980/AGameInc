using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingBarManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject startMenu;       // Assign your Start Menu container
    public GameObject titleBanner;     // Assign your TitleBanner object
    public GameObject loadingScreen;   // Assign your LoadingScreen panel
    public Slider loadingBar;          // Assign your LoadingBar slider

    [Header("Timing")]
    public float fillSpeed = 0.5f;
    public float postFillDelay = 5f;

    public void LoadSceneAsync(string sceneName)
    {
        // Hide Start Menu and Title Banner
        if (startMenu != null) startMenu.SetActive(false);
        if (titleBanner != null) titleBanner.SetActive(false);

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

        operation.allowSceneActivation = true;
    }
}
