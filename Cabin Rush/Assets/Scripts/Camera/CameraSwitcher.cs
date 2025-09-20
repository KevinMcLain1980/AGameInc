using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private Camera cinematicCamera;

    private void Start()
    {
        gameplayCamera.enabled = true;
        cinematicCamera.enabled = false;
    }

    public void ActivateCinematic()
    {
        gameplayCamera.enabled = false;
        cinematicCamera.enabled = true;
    }

    public void ReturnToGameplay()
    {
        cinematicCamera.enabled = false;
        gameplayCamera.enabled = true;
    }
}
