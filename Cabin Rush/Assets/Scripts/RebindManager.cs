using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindManager : MonoBehaviour
{
    public InputActionReference actionToRebind; // Drag the "Jump" action here in Inspector
    public TextMeshProUGUI bindingDisplay;
    public int bindingIndex = 0;

    private void Start()
    {
        LoadBinding(); // Load saved binding if available
    }

    public void StartRebind()
    {
        Debug.Log("Starting rebind...");

        actionToRebind.action.Disable();

        actionToRebind.action
            .PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") // optional: skip mouse
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                Debug.Log("Rebind complete: " + actionToRebind.action.bindings[0].effectivePath);
                operation.Dispose();
                actionToRebind.action.Enable();
                SaveBinding();
            })
            .Start();
    }

    private void SaveBinding()
    {
        var bindingOverride = actionToRebind.action.bindings[0].overridePath;
        PlayerPrefs.SetString(actionToRebind.action.id.ToString(), bindingOverride);
        PlayerPrefs.Save();
    }

    private void LoadBinding()
    {
        var bindingOverride = PlayerPrefs.GetString(actionToRebind.action.id.ToString(), string.Empty);
        if (!string.IsNullOrEmpty(bindingOverride))
        {
            actionToRebind.action.ApplyBindingOverride(0, bindingOverride);
        }

        void UpdateUI()
        {
            if (bindingDisplay != null && actionToRebind != null)
            {
                var binding = actionToRebind.action.bindings[bindingIndex];
                string display = actionToRebind.action.GetBindingDisplayString(bindingIndex);
                bindingDisplay.text = display;
            }
        }
    }

}
