using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatAttack : MonoBehaviour, IUnitAction
{
    public void ExecuteAction(Vector2Int targetPos, GridUnit actor, GridUnit targetUnit)
    {
        Debug.Log($"attacked o {targetUnit.name}");
    }
}
