using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] TacticalGridBuilder gridBuilder;

    [SerializeField] GridUnit[] allUnits;

    void Start()
    {
        gridBuilder.GenerateTacticalGrid();

        foreach (var unit in allUnits)
        {
            unit.SnapToClosestTile();
        }
    }
}
