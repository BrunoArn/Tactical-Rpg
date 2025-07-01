using System;
using System.Collections.Generic;
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

    void Start()
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
    }

    #region turn Logic

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
