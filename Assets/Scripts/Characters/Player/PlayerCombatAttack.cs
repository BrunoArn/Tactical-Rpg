using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatAttack : MonoBehaviour, IUnitAction
{
    //referencia para a classe gridUnit
    private GridUnit gridUnit;

     void Awake()
    {
        gridUnit = GetComponent<GridUnit>();
    }

    public void ExecuteAction(Vector2Int target)
    {
        GridUnit enemy = gridUnit.combatManager.unitPosition[target];
        Debug.Log($"attacked o {enemy.name}");
    }
}
