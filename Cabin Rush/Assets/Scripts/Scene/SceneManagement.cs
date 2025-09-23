using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private string sceneToLoad;

    public void TriggerSceneChange()
    {
        fadeAnimator.SetTrigger("FadeOut");
    }

    // Called at the end of the fade animation via Animation Event
    public void OnFadeComplete()
    {
        SceneManager.LoadScene("Cabin");
    }
}
