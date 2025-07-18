using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Gear/WeaponData")]
public class WeaponData : EquipmentItem
{
    [Header("Weapon Data")]
    public int damage;
    public int range;
    public int attackSpeed; //provavelmente relacionado ao maximo do fill meter
}
