using UnityEngine;

public static class SaveSystem
{
    public static SaveData Load(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);

        if (!System.IO.File.Exists(path))
            return null;

        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static string GetSlotPath(int slotIndex)
    {
        return Application.persistentDataPath + $"/save_slot_{slotIndex}.json";
    }

    public static int TotalSlotCount()
    {
        return 3; // Or however many slots you support
    }
}
