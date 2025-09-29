using UnityEngine;

public class LoadGamePage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject noSavesMessage;
    [SerializeField] private GameObject saveSlotsContainer;

    [Header("Save Slot Prefabs")]
    [SerializeField] private SaveSlot[] saveSlots; // Assign these in the inspector

    void Start()
    {
        RefreshSaveUI();
    }

    private void RefreshSaveUI()
    {
        bool hasValidSaves = false;

        foreach (var slot in saveSlots)
        {
            if (slot.HasValidSave()) // Your SaveSlot script should expose this
            {
                hasValidSaves = true;
                break;
            }
        }

        noSavesMessage.SetActive(!hasValidSaves);
        saveSlotsContainer.SetActive(hasValidSaves);
    }
}
