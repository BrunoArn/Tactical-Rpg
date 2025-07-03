using System;
using System.Collections.Generic;
using System.Linq;
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
    public Dictionary<Vector2Int, GridUnit> unitPosition = new();

    [Header("turn info")]
    [Space]
    //lista de turnos
    public List<TurnSlot> currentRound = new();
    //index to turno
    private int turnIndex = 0;
    //unidade atual no turno
    private GridUnit currentUnit;
    //ações totais por round
    [Tooltip("numero maximo de turnos por round")]
    [SerializeField] int totalActionsPerRound = 5;


    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        //pede pro builder gerar o grid
        gridBuilder.GenerateTacticalGrid();
        //detecta quem ta dentro do grid e joga pra lista
        DetectUnitsInGrid();
        //faz geral que ta na fight, entrar na fight se posicionando no grid
        foreach (var unit in allUnits)
        {
            unit.SnapToClosestTile();
        }
        //popula os personagens em suas posições
        CreatePositionDictionary();
        //gera o round e turnos
        GenerateRound();
        //começa os round e delega o primeiro a jogar
        startNextTurn();
    }

    #region turn Logic

    void GenerateRound()
    {
        currentRound.Clear();
        turnIndex = 0;
        //cria o dicionario para saber quantas vezes vagabundo vai bater
        Dictionary<GridUnit, float> fillMeters = new();
        //zera o fillmeter de geral
        foreach (GridUnit unit in allUnits) { fillMeters[unit] = 0f; }
        //inicia uma contagem da ordem
        int orderCounter = 0;
        //enquanto for menor que o limite de round, vai populando
        while (currentRound.Count < totalActionsPerRound)
        {
            //para cada unidade
            foreach (GridUnit unit in allUnits)
            {
                //vai enchendo o meters para meter ronca
                fillMeters[unit] += unit.stats.speed;
                // se passar ou chegar a 100, adiciona um turno
                if (fillMeters[unit] >= 100f)
                {
                    //adiciona o round com a ordem e soma a propria ordem tbm
                    currentRound.Add(new TurnSlot(unit, orderCounter++));
                    //diminui os 100
                    fillMeters[unit] -= 100f;
                    //sai se chegar no limite de rounds
                    if (currentRound.Count >= totalActionsPerRound) break;
                }
            }
        }
        //agora organiza o negocio
        currentRound = currentRound.OrderBy(t => t.order).ToList();
    }

    // começa setando quem tem que fazer oq e dps adiciona no index
    void startNextTurn()
    {
        if (turnIndex >= currentRound.Count)
        {
            GenerateRound(); // generates a new round
            turnIndex = 0;
        }

        currentUnit = currentRound[turnIndex].unit;
        Debug.Log($"{turnIndex+1}rd turno é do {currentUnit.name}");
        turnIndex++;

        //faz o proximo saber que tem que entrar
        // e da pra ele a funçao de terminar o turno por aqui tbm
        currentUnit.GetComponent<ICombatUnit>()?.StartTurn(EndCurrentTurn);
    }

    private void EndCurrentTurn()
    {
        startNextTurn();
    }

    //debug opara ver os round
    [ContextMenu("RoundOrder")]
    void DebugRoundOrder()
    {
        Debug.Log("=== Current Round Order ===");

        for (int i = 0; i < currentRound.Count; i++)
        {
            TurnSlot slot = currentRound[i];
            Debug.Log($"[{i}] {slot.unit.name}");
        }

        Debug.Log("===========================");
    }

    #endregion

    #region Detection Logic
    //detecta as unidades dentro do grid para adicionar a lista de unidades.
    void DetectUnitsInGrid()
    {
        //limpa a lista
        allUnits.Clear();
        //limpa lista de posições
        unitPosition.Clear();
        //pegar o tamanho do grid la na classe
        Bounds gridBounds = gridBuilder.GetGridBounds();
        //usa physics 2D para detectar collisao com as unidades no grid
        Collider2D[] hits = Physics2D.OverlapBoxAll(gridBounds.center, gridBounds.size, 0f, unitLayer);
        //verifica dentro deste array de hits cada coisa que pegou e se tem a classe GridUnit, se tiver, vai jogar
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<GridUnit>(out var unit))
                allUnits.Add(unit);
        }
    }

    private void CreatePositionDictionary()
    {
        //enche o dicionario de unidades
        foreach (GridUnit unit in allUnits)
        {
            //injeta o combatManager
            unit.combatManager = this;
            //adiciona no dicionario a unidade com sua possição no grid
            unitPosition[unit.currentGridPos] = unit;
        }
    }

    //debug pra ver quem ta ond 
    [ContextMenu("Unit position")]
    public void DebugUnitPositions()
    {
        Debug.Log(unitPosition.Count);
        foreach (var kvp in unitPosition)
        {
            Vector2Int pos = kvp.Key;
            GridUnit unit = kvp.Value;

            Debug.Log($"O {unit.name} está em: {pos}");
        }
    }
    #endregion
}
