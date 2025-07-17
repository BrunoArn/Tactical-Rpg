using UnityEngine;

public class EnemyMoveAction : MonoBehaviour, IUnitAction
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
    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        MoveCharacter(targetTile);
    }

    private void MoveCharacter(TileData target)
    {
        //joga a posição para a posição mundial do tile
        transform.position = target.worldPos;
        //atualiza o dicionário de posição
        gridUnit.UpdateGridPosition(target);
    }
}
