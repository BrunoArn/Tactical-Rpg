using UnityEngine;

public enum EquipmentSlotType { Weapon, Armor, Accessory }

[CreateAssetMenu(fileName = "NewGear", menuName = "Items/NewEquipment")]
public class EquipmentItem : ItemData
{
    [Header("Equipment Info")]
    public EquipmentSlotType slotType;
}
