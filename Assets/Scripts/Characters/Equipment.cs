using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private Dictionary<EquipmentSlotType, EquipmentItem> equippedGear = new();

    [Header("Só para testes, taca arma manualmente")]
    [SerializeField] EquipmentItem weapon;

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

    /////testeee
    [ContextMenu("Equip WEapon")]
    public void EquipWeapon()
    {
        equippedGear[weapon.slotType] = weapon;
    }
}
