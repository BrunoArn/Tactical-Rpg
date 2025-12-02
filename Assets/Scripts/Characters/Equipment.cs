using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private Dictionary<EquipmentSlotType, EquipmentItem> equippedGear = new();

    public EquipmentItem Equip(EquipmentItem item)
    {
        if (equippedGear.TryGetValue(item.slotType, out var previousItem))
        {
            equippedGear[item.slotType] = item;
            return previousItem;
        }
        equippedGear[item.slotType] = item;
        return null;
    }

    public EquipmentItem Unequip(EquipmentSlotType slotType)
    {
        if (equippedGear.TryGetValue(slotType, out var item))
        {
            equippedGear.Remove(slotType);
            return item;
        }
        return null;
    }

    public EquipmentItem GetEquippedItem(EquipmentSlotType slotType)
    {
        equippedGear.TryGetValue(slotType, out var equipped);
        return equipped;
    }

    //ta pegando somente a arma
    public int GearBonusDamage()
    {
        if (equippedGear.TryGetValue(EquipmentSlotType.Weapon, out var gear) && gear is WeaponData weapon)
        {
            return weapon.damage;
        }
        return 0;
    }

}
