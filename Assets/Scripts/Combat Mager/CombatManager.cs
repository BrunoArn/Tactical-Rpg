using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Grid info")]
    [Space]
    //referencia ao grid
    public TacticalGridBuilder gridBuilder;
    //vetor the unidades presentes no grid
    //serialized para ver no inspector de caozada
    [SerializeField] List<GridUnit> allUnits = new();
    private GridUnit hero;
    [SerializeField] List<GridObstacle> allObstacles = new();
    //o layer das units para procurar direitinho
    [Tooltip("layer dos personagens para encontrar e por no grid")]
    [SerializeField] LayerMask unitLayer;
    //dicionário de posição das unidades
    //public Dictionary<Vector2Int, GridUnit> unitPosition = new();

    [Header("turn info")]
    [Space]
    //cada um com a speed e tal
    [SerializeField] List<GridUnit> turnOrder;
    //index to turno
    private int turnIndex = 0;
    //unidade atual no turno
    //private GridUnit currentUnit;
    [Header("Game Events")]
    [SerializeField] GameEvent explorationRequest;


    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        //pede pro builder gerar o grid
        gridBuilder.StartGrid();
        //detecta quem ta dentro do grid e joga pra lista
        DetectUnitsInGrid();
        //faz geral que ta na fight, entrar na fight se posicionando no grid e da a grid pra eles
        PositionUnitsInGrid();
        //create o pathfinding la Flow-field
        gridBuilder.BuildFlowField(hero.currentTile.gridPos);

        // ensure single subscription for hero movement TESTE CARALHOOOO
        if (hero != null)
        {
            hero.OnUnitMove -= UpdatePathFinding;
            hero.OnUnitMove += UpdatePathFinding;
        }

        //gera o round e turnos
        GenerateRound();
        //começa os round e delega o primeiro a jogar
        StartNextTurn();
    }

    #region turn Logic

    //refatorar o nome
    void GenerateRound()
    {
        //pega todos os units
        turnOrder = allUnits;
        //ordena pelo speed
        turnOrder = turnOrder.OrderByDescending(unit => unit.stats.speed).ToList();
        //começa do zero
        turnIndex = 0;
    }

    // começa setando quem tem que fazer oq e dps adiciona no index
    void StartNextTurn()
    {
        //confere se terminou ou não
        if (turnIndex >= turnOrder.Count)
        {
            //zera se terminou
            turnIndex = 0;
        }
        //manda o cria startar a ação
        turnOrder[turnIndex].StartAction(EndCurrentTurn);

    }

    private void EndCurrentTurn()
    {
        turnIndex++;
        UpdatePathFinding();

        //check se acabou
        if (allUnits.Count == 1 && allUnits[0] == hero)
        {
            EndCombat();
        }
        else
            StartNextTurn();

    }

    private void RemoveUnit(GridUnit deadUnit)
    {
        allUnits.Remove(deadUnit);
        turnOrder.Remove(deadUnit);

        deadUnit.OnUnitDeath -= RemoveUnit;
        if (deadUnit == hero)
        {
            Debug.Log("Game over otario");
            EndCombat();
            return;
        }

        Destroy(deadUnit.gameObject);

    }

    private void RemoveObstacle(GridObstacle destroyedObstacle)
    {
        if (destroyedObstacle == null) return;
        // unsubscribe
        destroyedObstacle.OnObstacleDestruction -= RemoveObstacle;
        // remove from list and destroy gameObject
        allObstacles.Remove(destroyedObstacle);
        Destroy(destroyedObstacle.gameObject);
        // rebuild pathfinding
        if (hero != null)
            UpdatePathFinding();
    }

    private void UpdatePathFinding()
    {
        gridBuilder.BuildFlowField(hero.currentTile.gridPos);
    }

    public void EndCombat()
    {
        if (hero != null)
        {
            hero.OnUnitMove -= UpdatePathFinding;
        }

        if (gridBuilder != null)
        {
            gridBuilder.DestroyPathDistanceNumber();
        }

        // unsubscribe all unit/obstacle events to avoid duplicate subscriptions on re-detect
        foreach (var u in allUnits)
        {
            if (u != null)
                u.OnUnitDeath -= RemoveUnit;
        }

        foreach (var o in allObstacles)
        {
            if (o != null)
                o.OnObstacleDestruction -= RemoveObstacle;
        }

        allUnits.Clear();
        allObstacles.Clear();
        // reset hero reference so next encounter starts clean
        hero = null;
        explorationRequest.Raise();
    }

    #endregion

    #region Detection Logic
    //detecta as unidades dentro do grid para adicionar a lista de unidades.
    void DetectUnitsInGrid()
    {
        // get grid bounds and overlaps
        Bounds gridBounds = gridBuilder.GetGridBounds();
        //usa physics 2D para detectar collisao com as unidades no grid
        Collider2D[] hits = Physics2D.OverlapBoxAll(gridBounds.center, gridBounds.size, 0f, unitLayer);

        // collect unique results first (OverlapBoxAll may return multiple colliders per unit)
        var foundUnits = new List<GridUnit>();
        var foundObstacles = new List<GridObstacle>();

        // When colliders are on child objects, use GetComponentInParent to find the
        // logical unit/obstacle component. Also deduplicate by instance id because
        // a single unit/obstacle may produce multiple collider hits.
        var foundUnitIds = new HashSet<int>();
        var foundObstacleIds = new HashSet<int>();

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            var unit = hit.transform.GetComponentInParent<GridUnit>();
            if (unit != null)
            {
                var id = unit.GetInstanceID();
                if (foundUnitIds.Add(id))
                    foundUnits.Add(unit);
            }

            var obstacle = hit.transform.GetComponentInParent<GridObstacle>();
            if (obstacle != null)
            {
                var id = obstacle.GetInstanceID();
                if (foundObstacleIds.Add(id))
                    foundObstacles.Add(obstacle);
            }
        }

        // Unsubscribe old handlers from current lists to avoid duplicates
        foreach (var u in allUnits)
        {
            if (u != null)
                u.OnUnitDeath -= RemoveUnit;
        }
        foreach (var o in allObstacles)
        {
            if (o != null)
                o.OnObstacleDestruction -= RemoveObstacle;
        }

        // replace lists with the deduplicated found lists
        allUnits.Clear();
        allObstacles.Clear();
        hero = null;

        // assign found units and subscribe handlers once
        foreach (var gUnit in foundUnits)
        {
            if (gUnit == null) continue;

            if (gUnit.CompareTag("Player"))
                hero = gUnit;

            // ensure single subscription
            gUnit.OnUnitDeath -= RemoveUnit;
            gUnit.OnUnitDeath += RemoveUnit;

            allUnits.Add(gUnit);
        }

        // assign found obstacles and subscribe handlers once
        foreach (var gObs in foundObstacles)
        {
            if (gObs == null) continue;

            gObs.OnObstacleDestruction -= RemoveObstacle;
            gObs.OnObstacleDestruction += RemoveObstacle;

            allObstacles.Add(gObs);
        }

        // ensure hero movement handler is only subscribed once
        if (hero != null)
        {
            hero.OnUnitMove -= UpdatePathFinding;
            hero.OnUnitMove += UpdatePathFinding;
        }
    }


    private void PositionUnitsInGrid()
    {
        foreach (GridUnit unit in allUnits)
        {
            //fazendo isso no roleplay total, atenção!!
            //adicionando o player só no drop loot dos cara, vamos ver como tirar isso depois.
            if (unit.CompareTag("Enemy")) unit.GetComponent<PickUpSpawner>().player = hero.transform.root.gameObject;

            // posição atual da unidade, pode estar fora do grid
            Vector3 currentPos = unit.transform.root.position;
            //montar a comparação de distancia
            // começa com infinito para que a primeira seja sempre suave
            float closestDist = Mathf.Infinity;
            Vector2Int closestKey = Vector2Int.zero;

            //verifica todos os tiles do grid e descobre qual o mais perto
            foreach (var tileEntry in gridBuilder.tacticalGrid)
            {
                float dist = Vector3.Distance(currentPos, tileEntry.Value.worldPos);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestKey = tileEntry.Key;
                }
            }
            // se achou um tile, snap
            if (gridBuilder.tacticalGrid.TryGetValue(closestKey, out var tileData))
            {
                unit.transform.root.position = tileData.worldPos;
                ///// ================== isso aqui pdoe ser o TIle direto ==================
                unit.UpdateGridPosition(tileData);
            }
        }

        foreach (GridObstacle obstacle in allObstacles)
        {
            obstacle.GetComponent<PickUpSpawner>().player = hero.transform.root.gameObject;

            if (obstacle == null) continue;
            // find closest tile to obstacle's root position
            Vector3 obsPos = obstacle.transform.root.position;
            float closestDist = Mathf.Infinity;
            Vector2Int closestKey = Vector2Int.zero;

            foreach (var tileEntry in gridBuilder.tacticalGrid)
            {
                float dist = Vector3.Distance(obsPos, tileEntry.Value.worldPos);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestKey = tileEntry.Key;
                }
            }

            if (gridBuilder.tacticalGrid.TryGetValue(closestKey, out var tileData))
            {
                obstacle.currentTile = tileData;
                tileData.OccupyingObstacle = obstacle;
                tileData.isWalkable = false;
                // optionally snap obstacle to tile world pos
                obstacle.transform.root.position = tileData.worldPos;
            }
        }
    }


    //debug pra ver quem ta ond 
    [ContextMenu("Unit position")]
    public void DebugUnitPositions()
    {
        Debug.Log(allUnits.Count);
        foreach (var kvp in allUnits)
        {

            Debug.Log($"O {kvp.name} está em: {kvp.currentTile.gridPos}");
        }
    }
    #endregion
}
