using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    //referencia ao grid
    [SerializeField] TacticalGridBuilder gridBuilder;

    //vetor the unidades presentes no grid, talvez devesse identificar por si só
    [SerializeField] List<GridUnit> allUnits = new();
    [SerializeField] LayerMask unitLayer;

    void Start()
    {
        gridBuilder.GenerateTacticalGrid();
        DetectUnitsInGrid();

        foreach (var unit in allUnits)
        {
            unit.SnapToClosestTile();
        }
    }

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
}
