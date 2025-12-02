using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private Dictionary<EquipmentSlotType, EquipmentItem> equippedGear = new();

    public EquipmentItem Equip(EquipmentItem item)
    {
        if(equippedGear.TryGetValue(item.slotType, out var previousItem))
        {
            equippedGear[item.slotType] = item;
            return previousItem;
        }
        equippedGear[item.slotType] = item;
        return null;
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
