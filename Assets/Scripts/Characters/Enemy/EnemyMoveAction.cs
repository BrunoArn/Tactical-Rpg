using System.Collections;
using UnityEngine;

public class EnemyMoveAction : MonoBehaviour, IUnitAction
{
    //referencia para a classe gridUnit
    private GridUnit gridUnit;
    [Header("Animação")]
    [SerializeField] private Animator myAnimator;
    [SerializeField] private AnimationClip walkAnimation;
    private ICombatUnit actionController;
    private float moveDuration;
    
    //direção que vai ser feito o movimento
    private Vector2Int direction = Vector2Int.zero;


    void Awake()
    {
        gridUnit = GetComponent<GridUnit>();
        moveDuration = walkAnimation.length;
    }

    //executa a ação, neste caso é se mover.
    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        //MoveCharacter(targetTile);
        actionController = actor.genericActionController;
        StartCoroutine(MoveCharacterRoutine(targetTile));
    }

    private void MoveCharacter(TileData target)
    {
        //joga a posição para a posição mundial do tile
        transform.position = target.worldPos;
        //atualiza o dicionário de posição
        gridUnit.UpdateGridPosition(target);
    }

    private IEnumerator MoveCharacterRoutine(TileData target)
    {
        myAnimator.SetBool("isMoving", true);

        Vector3 start = transform.position;
        Vector3 end = target.worldPos;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        myAnimator.SetBool("isMoving", false);

        gridUnit.UpdateGridPosition(target);
        actionController.BeforeEndTurn();
    }
}
