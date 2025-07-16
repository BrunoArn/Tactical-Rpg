using System;
using UnityEngine;

public class GridUnit : MonoBehaviour
{

    // é injetado pelo próprio combatManager;
    public TileData currentTile;

    // status
    public UnitStats stats;
    public Health health;

    //receeb a controller do cara, para poder startar pelo comat manager
    [SerializeField] MonoBehaviour actionController;
    //fazer ser a interface
    private ICombatUnit action;

    //esse é o evento para o comatManager receber
    public event Action<GridUnit> OnUnitDeath;

    void Awake()
    {
        //setando o controller que receer independentemente
        action = actionController as ICombatUnit;

        //me relacionando ao evento da morte do health
        health.OnDeath += HandleDeath;
    }

    //update grid position
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

    //startt unit action
    public void StartAction(System.Action onTurnEndCallBack)
    {
        //primeira call do actionController pro turno
        action.BeforeStart(onTurnEndCallBack);
    }

    private void HandleDeath()
    {
        OnUnitDeath?.Invoke(this);
        currentTile.ClearTile();
        Destroy(this.gameObject);
    }
}
