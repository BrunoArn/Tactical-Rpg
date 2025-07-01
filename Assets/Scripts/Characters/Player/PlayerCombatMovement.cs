using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCombatMovement : MonoBehaviour, IUnitAction
{
    //referencia para a classe gridUnit
    private GridUnit gridUnit;
    //direção que vai ser feito o movimento
    private Vector2Int direction = Vector2Int.zero;

    void Awake()
    {
        gridUnit = GetComponent<GridUnit>();
    }

    //executa a ação, neste caso é se mover.
    public void ExecuteAction(Vector2Int target)
    {
        if (gridUnit.gridBuilder.tacticalGrid.TryGetValue(target, out var tile) && tile.isWalkable)
        {
            //joga a posição para a posição mundial do tile
            transform.position = tile.worldPos;
            //atualiza o dicionário de posição
            gridUnit.UpdateGridPosition(target);
        }
    }
}
