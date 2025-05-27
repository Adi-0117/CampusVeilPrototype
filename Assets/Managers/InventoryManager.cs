using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform itemsContainer;
    public GameObject slotPrefab;

    private List<ItemData> items = new List<ItemData>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Call this to add an item at runtime:
    public void AddItem(ItemData newItem)
    {
        items.Add(newItem);
        RefreshUI();
    }

    // Call this to remove after use:
    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        RefreshUI();
    }

    void RefreshUI()
    {
        // Clear old slots
        foreach (Transform t in itemsContainer) Destroy(t.gameObject);

        // Rebuild
        foreach (var item in items)
        {
            var slot = Instantiate(slotPrefab, itemsContainer);
            var icon = slot.transform.Find("IconImage").GetComponent<UnityEngine.UI.Image>();
            icon.sprite = item.icon;

            // Hook up the button to "Use" this item
            slot.GetComponent<UnityEngine.UI.Button>()
                .onClick.AddListener(() => OnUseItem(item));
        }
    }

    void OnUseItem(ItemData item)
    {
        Debug.Log($"Using item: {item.itemName}");
        // TODO: apply item effect (e.g., open locked door)
        RemoveItem(item);
    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    public void ToggleInventory()
    {
        bool isOpen = inventoryPanel.activeSelf;
        if (!isOpen)
            RefreshUI();                // rebuild slots before showing
        inventoryPanel.SetActive(!isOpen);
    }
}
