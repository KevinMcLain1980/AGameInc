using UnityEngine;

public class SaveSlotManager : MonoBehaviour
{
    [Header("Prefab Setup")]
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private Transform saveSlotsContainer;

    [Header("UI References")]
    [SerializeField] private GameObject noSavesMessage;

    private void Start()
    {
        GenerateSlots();
        RefreshUI();
    }

    private void GenerateSlots()
    {
        if (saveSlotPrefab == null || saveSlotsContainer == null)
        {
            Debug.LogError("SaveSlotManager is missing prefab or container reference.");
            return;
        }

        int totalSlots = SaveSystem.TotalSlotCount();

        for (int i = 0; i < totalSlots; i++)
        {
            Debug.Log($"Instantiating slot {i}");

            GameObject slotGO = Instantiate(saveSlotPrefab, saveSlotsContainer);
            slotGO.transform.localScale = Vector3.one;
            slotGO.SetActive(true);

            SaveSlot slot = slotGO.GetComponent<SaveSlot>();
            if (slot != null)
            {
                slot.LoadSlotData(i);
            }
            else
            {
                Debug.LogWarning($"SaveSlot component missing on prefab instance {i}");
            }
        }
    }

    private void RefreshUI()
    {
        bool hasValidSave = false;

        foreach (Transform child in saveSlotsContainer)
        {
            SaveSlot slot = child.GetComponent<SaveSlot>();
            if (slot != null && slot.HasValidSave())
            {
                hasValidSave = true;
                break;
            }
        }

        noSavesMessage.SetActive(!hasValidSave);
        saveSlotsContainer.gameObject.SetActive(hasValidSave);
    }
}
