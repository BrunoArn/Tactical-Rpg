using UnityEngine;

public class EnemyAttackAction : MonoBehaviour, IUnitAction
{
    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        if (targetTile.OccupyingUnit == null) return;

        targetTile.OccupyingUnit.health.TakeDamage(actor.stats.attack);
    }
}
