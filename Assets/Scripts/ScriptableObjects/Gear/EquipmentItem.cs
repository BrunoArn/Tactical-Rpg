using UnityEngine;

public enum EquipmentSlotType { Weapon, Armor, Accessory }

[CreateAssetMenu(fileName = "NewGear", menuName = "Gear/EquipmentItem")]
public class EquipmentItem : ScriptableObject
{
    [Header("Gear info")]
    public string itemName;
    public EquipmentSlotType slotType;

    [Header("Gear Visual")]
    public Sprite icon;
}
