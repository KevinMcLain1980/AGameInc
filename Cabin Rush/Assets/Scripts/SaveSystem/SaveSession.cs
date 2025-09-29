using UnityEngine;

public static class SaveSession
{
    // Holds the currently loaded save data
    public static SaveData CurrentSave { get; private set; }

    // Optional: Track which slot was loaded
    public static int CurrentSlotIndex { get; private set; }

    // Call this when loading a save
    public static void Initialize(SaveData data, int slotIndex)
    {
        CurrentSave = data;
        CurrentSlotIndex = slotIndex;
    }

    // Optional: Clear session data (e.g., on logout or new game)
    public static void Clear()
    {
        CurrentSave = null;
        CurrentSlotIndex = -1;
    }

    // Optional: Check if a session is active
    public static bool HasActiveSession()
    {
        return CurrentSave != null && CurrentSave.IsValid();
    }
}
