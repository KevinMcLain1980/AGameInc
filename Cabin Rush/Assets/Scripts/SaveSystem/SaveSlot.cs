using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private TextMeshProUGUI timestampText;
    [SerializeField] private Button loadButton;

    private SaveData saveData;
    private int slotIndex;

    public void LoadSlotData(int index)
    {
        slotIndex = index;
        saveData = SaveSystem.Load(slotIndex);

        if (HasValidSave())
        {
            UpdateUI();
            loadButton.interactable = true;
        }
        else
        {
            ClearUI();
            loadButton.interactable = false;
        }
    }

    public bool HasValidSave()
    {
        return saveData != null && saveData.IsValid();
    }

    private void UpdateUI()
    {
        playerNameText.text = saveData.playerName;
        levelText.text = $"Level {saveData.level}";
        playTimeText.text = FormatPlayTime(saveData.playTime);
        timestampText.text = saveData.saveTimestamp;
        Debug.Log($"Slot {slotIndex} loaded: {saveData?.playerName}");
    }

    private void ClearUI()
    {
        playerNameText.text = "Empty Slot";
        levelText.text = "";
        playTimeText.text = "";
        timestampText.text = "";
    }

    private string FormatPlayTime(float seconds)
    {
        int hrs = Mathf.FloorToInt(seconds / 3600);
        int mins = Mathf.FloorToInt((seconds % 3600) / 60);
        return $"{hrs}h {mins}m";
    }

    public void OnLoadButtonPressed()
    {
        if (HasValidSave())
        {
            // Load the game using this slot's data
            GameLoader.LoadFromSlot(slotIndex); // Replace with your actual game loading logic
        }
    }
}
