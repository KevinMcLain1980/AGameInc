using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour
{
    public float fadeDuration = 2f;
    private AudioSource audioSource;
    private bool fadingOut = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Define which scenes are gameplay scenes
        if (scene.name == "Cabin" || scene.name == "Grand Dining Hall" || scene.name == "Deck")
        {
            if (!fadingOut)
                StartCoroutine(FadeOutAndDestroy());
        }
    }

    System.Collections.IEnumerator FadeOutAndDestroy()
    {
        fadingOut = true;
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        Destroy(gameObject);
    }
}
