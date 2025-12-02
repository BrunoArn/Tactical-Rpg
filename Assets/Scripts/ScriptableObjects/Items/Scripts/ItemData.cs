using UnityEngine;

public enum ItemType { Equipment, Consumable, Material, Quest }

[CreateAssetMenu(fileName = "newItem", menuName = "Items/newItem")]
public class ItemData : ScriptableObject
{
    [Header("Item info")]
    public string itemName;
    public ItemType itemType;
    public bool isStackable = true;

    [Header("Item Visual")]
    public Sprite icon;
    
}
