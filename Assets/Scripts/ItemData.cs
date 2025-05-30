// ItemData.cs
// ScriptableObject defining an inventory item’s properties.
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [TextArea]
    public string description;
}
