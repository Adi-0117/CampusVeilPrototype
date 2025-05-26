using UnityEngine;

public class Pickup : MonoBehaviour
{
    public ItemData item;  // assign in Inspector

    void OnMouseDown()
    {
        InventoryManager.Instance.AddItem(item);
        Destroy(gameObject);
    }
}
