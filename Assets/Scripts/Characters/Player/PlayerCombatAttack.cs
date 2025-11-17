using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatAttack : MonoBehaviour, IUnitAction
{
    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        // target could be a unit or an obstacle
        if (targetTile == null) return;

        int baseAttack = actor.stats.attack;
        int weaponDamage = actor.equips.GearBonusDamage();
        int totalDamage = baseAttack + weaponDamage;

        if (targetTile.OccupyingUnit != null)
        {
            Debug.Log($"Ataquei o [{targetTile.OccupyingUnit.name}]");
            if (targetTile.OccupyingUnit.health != null)
            {
                targetTile.OccupyingUnit.health.TakeDamage(totalDamage);
            }
        }

        if (targetTile.OccupyingObstacle != null)
        {
            Debug.Log($"Ataquei o obstáculo [{targetTile.OccupyingObstacle.name}]");
            if (targetTile.OccupyingObstacle.health != null)
            {
                targetTile.OccupyingObstacle.health.TakeDamage(totalDamage);
            }
        }
    }
}
