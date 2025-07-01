using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    //referencia ao grid
    [SerializeField] TacticalGridBuilder gridBuilder;
    //vetor the unidades presentes no grid
    [SerializeField] List<GridUnit> allUnits = new();
    //o layer das units para procurar direitinho
    [SerializeField] LayerMask unitLayer;
    //dicionário de posição das unidades
    public Dictionary<Vector2Int, GridUnit> unitPosition = new();


    //turn things
    public List<TurnSlot> currentRound = new();
    private int turnIndex = 0;

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
        //printa as posiçoes
        DebugUnitPositions();

        GenerateRound();
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

        int orderCounter = 0;

        while (currentRound.Count < totalActionsPerRound)
        {
            foreach (GridUnit unit in allUnits)
            {
                fillMeters[unit] += unit.stats.speed;

                if (fillMeters[unit] >= 100f)
                {
                    currentRound.Add(new TurnSlot(unit, orderCounter++));
                    fillMeters[unit] -= 100f;

                    if (currentRound.Count >= totalActionsPerRound) break;
                }
            }
        }



        //agora organiza o negocio
        currentRound = currentRound.OrderBy(t => t.order).ToList();
        DebugRoundOrder();
    }

    //debug opara ver os round
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
    public void DebugUnitPositions()
    {
        foreach (var kvp in unitPosition)
        {
            Vector2Int pos = kvp.Key;
            GridUnit unit = kvp.Value;

            Debug.Log($"O {unit.name} está em: {pos}");
        }
    }
    #endregion
}
