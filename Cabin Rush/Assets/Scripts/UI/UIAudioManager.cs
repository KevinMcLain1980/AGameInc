using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHover()
    {
        if (hoverSound != null) audioSource.PlayOneShot(hoverSound);
    }

    public void PlayClick()
    {
        if (clickSound != null) audioSource.PlayOneShot(clickSound);
    }
}
