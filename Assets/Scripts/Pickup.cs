// Pickup.cs
// Adds an item to inventory when the object is clicked.
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public ItemData item;

    void OnMouseDown()
    {
        InventoryManager.Instance.AddItem(item);
        Destroy(gameObject);
    }
}
