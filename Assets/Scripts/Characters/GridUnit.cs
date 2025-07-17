using System;
using UnityEngine;

public class GridUnit : MonoBehaviour
{

    // é injetado pelo próprio combatManager;
    public TileData currentTile;

    [Header("Status")]
    public UnitStats stats;
    public Health health;
    [SerializeField] HealthUi healthBar;
    public Equipment equips;

    [Header("Actions")]
    //receeb a controller do cara, para poder startar pelo comat manager
    [SerializeField] MonoBehaviour actionController;
    //fazer ser a interface
    private ICombatUnit genericActionController;

    //esse é o evento para o comatManager receber que o cara morreu
    public event Action<GridUnit> OnUnitDeath;
    public event Action OnUnitMove;

    void Awake()
    {
        //setando o controller que receer independentemente
        genericActionController = actionController as ICombatUnit;

        //me relacionando ao eventos de Health
        health.OnDeath += HandleDeath;
        health.OnTakeDamage += UpdateHealthBar;
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
        OnUnitMove?.Invoke();
    }

    //startt unit action
    public void StartAction(System.Action onTurnEndCallBack)
    {
        //primeira call do actionController pro turno
        genericActionController.BeforeStart(onTurnEndCallBack);
    }

    private void HandleDeath()
    {
        OnUnitDeath?.Invoke(this);
        currentTile.ClearTile();
        Destroy(this.gameObject);
    }

    private void UpdateHealthBar(int current, int max)
    {
        healthBar.ShowHealthBar();
        healthBar.SetHealthValue(current, max);
    }
}
