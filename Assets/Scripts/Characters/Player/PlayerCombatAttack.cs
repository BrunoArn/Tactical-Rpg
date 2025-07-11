using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatAttack : MonoBehaviour, IUnitAction
{
    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        Debug.Log($"attacked o {targetTile.OccupyingUnit.name}");
    }
}
