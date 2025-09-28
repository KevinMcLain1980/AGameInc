using System;

[Serializable]
public class SaveData
{
    // Basic player info
    public string playerName;
    public int level;
    public float playTime; // in seconds

    // Optional stats for expansion
    public int coinsCollected;
    public int hazardsSurvived;
    public string lastSceneName;

    // Timestamp for sorting or display
    public string saveTimestamp;

    // Validation logic to check if this save is usable
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(playerName) && level > 0 && !string.IsNullOrEmpty(saveTimestamp);
    }

    // Optional: Constructor for manual creation
    public SaveData(string name, int lvl, float time, int coins, int hazards, string scene)
    {
        playerName = name;
        level = lvl;
        playTime = time;
        coinsCollected = coins;
        hazardsSurvived = hazards;
        lastSceneName = scene;
        saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
