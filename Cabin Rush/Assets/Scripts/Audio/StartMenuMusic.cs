using UnityEngine;

public class StartMenuMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
