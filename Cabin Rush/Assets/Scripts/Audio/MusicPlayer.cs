using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private void Start()
    {
        GetComponent<AudioSource>().Play();
    }
}
