using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatAttack : MonoBehaviour, IUnitAction
{
    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        if (targetTile.OccupyingUnit == null) return;
        Debug.Log($"Ataquei o [{targetTile.OccupyingUnit.name}]");

        int baseAttack = actor.stats.attack;
        int weaponDamage = actor.equips.GearBonusDamage();
        int totalDamage = baseAttack + weaponDamage;

        targetTile.OccupyingUnit.health.TakeDamage(totalDamage);
    }
}
