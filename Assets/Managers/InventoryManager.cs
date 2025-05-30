// InventoryManager.cs
// Manages player inventory, UI slots, and item use/removal.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddItem(ItemData newItem)
    {
        items.Add(newItem);
        RefreshUI();
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        RefreshUI();
    }

    void RefreshUI()
    {
        foreach (Transform t in itemsContainer)
            Destroy(t.gameObject);

        foreach (var item in items)
        {
            var slot = Instantiate(slotPrefab, itemsContainer);
            var icon = slot.transform.Find("IconImage").GetComponent<Image>();
            icon.sprite = item.icon;
            slot.GetComponent<Button>().onClick.AddListener(() => OnUseItem(item));
        }
    }

    void OnUseItem(ItemData item)
    {
        Debug.Log($"Using item: {item.itemName}");
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
            RefreshUI();
        inventoryPanel.SetActive(!isOpen);
    }
}
