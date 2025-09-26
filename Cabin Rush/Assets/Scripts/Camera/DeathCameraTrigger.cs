using UnityEngine;

public class DeathCameraTrigger : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var switcher = Object.FindAnyObjectByType<CameraSwitcher>();
        if (switcher != null)
        {
            switcher.ActivateCinematic();
        }
    }
}
