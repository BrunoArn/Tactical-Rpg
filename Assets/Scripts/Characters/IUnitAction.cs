using UnityEngine;

public interface IUnitAction
{
    void ExecuteAction(TileData targetTile, GridUnit actor);
}
