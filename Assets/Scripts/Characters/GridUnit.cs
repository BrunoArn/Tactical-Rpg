using UnityEngine;

public class GridUnit : MonoBehaviour
{

    // é injetado pelo próprio combatManager;
    public TileData currentTile;

    // status
    public UnitStats stats;

    //receeb a controller do cara, para poder startar pelo comat manager
    [SerializeField] MonoBehaviour actionController;
    //fazer ser a interface
    private ICombatUnit action;

    void Awake()
    {
        action = actionController as ICombatUnit;
    }

    public void UpdateGridPosition(TileData newTile)
    {
        if (currentTile == null)
        {
            currentTile = newTile;
            currentTile.OccupyingUnit = this;
        }
        else
        {
            currentTile.ClearTile();
            currentTile = newTile;
            currentTile.OccupyingUnit = this;
        }
    }

    public void StartAction(System.Action onTurnEndCallBack)
    {
        //primeira call do actionController pro turno
        action.BeforeStart(onTurnEndCallBack);
    }
}
