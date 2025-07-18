using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemyActionController : MonoBehaviour, ICombatUnit
{
    private System.Action onTurnEnd;

    private GridUnit gridUnit;

    [Header("actions")]
    [SerializeField] MonoBehaviour MoveAction;
    [SerializeField] MonoBehaviour AttackAction;
    private IUnitAction moveAction;
    private IUnitAction attackAction;

    void Awake()
    {
        gridUnit = GetComponent<GridUnit>();
        moveAction = MoveAction as IUnitAction;
        attackAction = AttackAction as IUnitAction;
    }

    public void BeforeStart(System.Action onTurnEndCallBack)
    {
        //manda pro endturn a funcao do comatmanager
        onTurnEnd = onTurnEndCallBack;
        //adiciona o meter e avisa que aumentou
        gridUnit.stats.AddMeter(gridUnit.stats.speed);
        if (gridUnit.stats.Meter >= gridUnit.stats.MeterMax)
        {
            StartTurn();
        }
        else
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        if (gridUnit.currentTile.distanceToHero <= gridUnit.stats.range)
        {
            //attack
            var heroTile = gridUnit.currentTile.neighbors.FirstOrDefault(t => t.distanceToHero == 0 && t.OccupyingUnit != null);
            if (heroTile != null && attackAction != null)
            {
                attackAction.ExecuteAction(heroTile, gridUnit);
            }
        }
        else
        {
            TryToMoveAlongPath();
        }
        BeforeEndTurn();
    }

    public void BeforeEndTurn()
    {
        gridUnit.stats.AddMeter(-gridUnit.stats.MeterMax);
        EndTurn();

    }

    public void EndTurn()
    {
        onTurnEnd?.Invoke(); // manda pro manager que ta tudo bem
    }

    private void TryToMoveAlongPath()
    {
        // var direction = gridUnit.currentTile.preferredDirection;
        // if (direction == Vector2Int.zero) return;

        // var nextTile = gridUnit.currentTile.neighbors.FirstOrDefault(t => t.gridPos == gridUnit.currentTile.gridPos + direction);
        // if (nextTile != null && nextTile.isWalkable && nextTile.OccupyingUnit == null && moveAction != null)
        // {
        //     moveAction.ExecuteAction(nextTile, gridUnit);
        // }

        var sorted = gridUnit.currentTile.neighbors.OrderBy(tile => tile.distanceToHero);
        var fallback = sorted.FirstOrDefault(tile => tile.isWalkable && tile.OccupyingUnit == null);

        if (fallback != null)
        {
            moveAction.ExecuteAction(fallback, gridUnit);
        }
    }

}
