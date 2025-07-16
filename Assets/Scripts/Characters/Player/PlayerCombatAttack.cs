using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatAttack : MonoBehaviour, IUnitAction
{
    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        Debug.Log($"Ataquei o [{targetTile.OccupyingUnit.name}]");
        targetTile.OccupyingUnit.health.TakeDamage(5);
    }
}
