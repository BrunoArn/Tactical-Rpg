using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatRangedAttack : MonoBehaviour, IUnitAction, IRangedAction
{
    [SerializeField] private int maxRange = 3;

    public int MaxRange => maxRange;

    // Find first occupied tile in direction up to maxRange
    public TileData FindTarget(GridUnit origin, Vector2Int dir)
    {
        if (origin == null || origin.currentTile == null) return null;
        TileData t = origin.currentTile;
        for (int i = 0; i < maxRange; i++)
        {
            t = t.GetNeighbors(dir);
            if (t == null) return null;
            if (t.IsOccupied) return t;
        }
        return null;
    }


    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        if (targetTile == null || actor == null) return;

        int baseAttack = actor.stats.attack;
        int weaponDamage = actor.equips.GearBonusDamage();
        int totalDamage = baseAttack + weaponDamage;

        if (targetTile.OccupyingUnit != null)
        {
            if (targetTile.OccupyingUnit.health != null)
            {
                targetTile.OccupyingUnit.health.TakeDamage(totalDamage);
            }
        }

        if (targetTile.OccupyingObstacle != null)
        {
            if (targetTile.OccupyingObstacle.health != null)
            {
                targetTile.OccupyingObstacle.health.TakeDamage(totalDamage);
            }
        }
    }
}
