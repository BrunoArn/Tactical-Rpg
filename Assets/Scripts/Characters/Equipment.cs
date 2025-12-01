using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private Dictionary<EquipmentSlotType, EquipmentItem> equippedGear = new();

    public void Equip(EquipmentItem item)
    {
        equippedGear[item.slotType] = item;
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
