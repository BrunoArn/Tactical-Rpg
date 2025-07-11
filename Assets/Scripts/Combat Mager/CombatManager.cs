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


    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        //pede pro builder gerar o grid
        gridBuilder.StartGrid();
        //detecta quem ta dentro do grid e joga pra lista
        DetectUnitsInGrid();
        //faz geral que ta na fight, entrar na fight se posicionando no grid e da a grid pra eles
        PositionUnitsInGrid();




        //gera o round e turnos
        GenerateRound();
        //começa os round e delega o primeiro a jogar
        startNextTurn();
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
    void startNextTurn()
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
        startNextTurn();
    }

    #endregion

    #region Detection Logic
    //detecta as unidades dentro do grid para adicionar a lista de unidades.
    void DetectUnitsInGrid()
    {
        //limpa a lista
        allUnits.Clear();
        //pegar o tamanho do grid la na classe
        Bounds gridBounds = gridBuilder.GetGridBounds();
        //usa physics 2D para detectar collisao com as unidades no grid
        Collider2D[] hits = Physics2D.OverlapBoxAll(gridBounds.center, gridBounds.size, 0f, unitLayer);
        //verifica dentro deste array de hits cada coisa que pegou e se tem a classe GridUnit, se tiver, vai jogar
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<GridUnit>(out var unit))
            {
                allUnits.Add(unit);
            }

        }
    }

    private void PositionUnitsInGrid()
    {
        foreach (GridUnit unit in allUnits)
        {
            // posição atual da unidade, pode estar fora do grid
            Vector3 currentPos = unit.transform.position;
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
                unit.transform.position = tileData.worldPos;

                ///// ================== isso aqui pdoe ser o TIle direto ==================
                unit.UpdateGridPosition(tileData);
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
