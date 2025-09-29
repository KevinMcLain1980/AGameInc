using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameLoader
{
    public static void LoadFromSlot(int slotIndex)
    {
        SaveData data = SaveSystem.Load(slotIndex);

        if (data == null || !data.IsValid())
        {
            Debug.LogWarning($"No valid save data found in slot {slotIndex}");
            return;
        }

        // Optionally store the data for use in the next scene
        SaveSession.Initialize(data, slotIndex);

        // Load the saved scene
        SceneManager.LoadScene(data.lastSceneName);
    }
}
