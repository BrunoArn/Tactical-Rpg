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
        MoveCharacter(target);
    }

    private void MoveCharacter(Vector2Int target)
    {
        //joga a posição para a posição mundial do tile
        transform.position = gridUnit.gridBuilder.tacticalGrid[target].worldPos;
        //atualiza o dicionário de posição
        gridUnit.UpdateGridPosition(target);
    }
}
