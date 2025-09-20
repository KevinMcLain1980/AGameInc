using UnityEngine;

public class PlayerAnimatorSync : MonoBehaviour
{
    private Animator animator;

    private void Awake() => animator = GetComponent<Animator>();

    public void SetTrigger(string triggerName)
    {
        if (animator) animator.SetTrigger(triggerName);
    }

    public void SetBool(string boolName, bool value)
    {
        if (animator) animator.SetBool(boolName, value);
    }

    public void SetFloat(string floatName, float value)
    {
        if (animator) animator.SetFloat(floatName, value);
    }
}
