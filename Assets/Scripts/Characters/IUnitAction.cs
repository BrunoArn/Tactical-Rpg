using UnityEngine;

public interface IUnitAction
{
    void ExecuteAction(Vector2Int targetPos, GridUnit actor, GridUnit targetUnit);
}
