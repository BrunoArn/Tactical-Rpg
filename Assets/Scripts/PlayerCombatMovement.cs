using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCombatMovement : MonoBehaviour
{
    //referencia para a classe gridUnit
    private GridUnit gridUnit;
    //direção que vai ser feito o movimento
    private Vector2Int direction = Vector2Int.zero;
    //flag para ver se ja andou ou não.
    private bool hasMoved = false;

    //meter um highlight de ond vai sair a andada
    [SerializeField] private GameObject HighlightPrefab;
    private GameObject highlightInstance;
    //os controles
    private CombatControls controls;

    void Awake()
    {
        //pega o componente do grid Unit e pega os controles
        gridUnit = GetComponent<GridUnit>();
        controls = new CombatControls();

        //ctx é contexto
        //aqui ele pega os input e só executa em determinados contextos
        //esse no caso é da direção da movimentação
        controls.Combat.Move.performed += ctx =>
        {
            //se nja se mexeu, nem começa
            if (hasMoved) return;
            //está lendo o valor para vector2, pois é um input de cima, baixao , esquerda e direita
            Vector2 input = ctx.ReadValue<Vector2>();
            //aplica na direção desejada
            direction = new Vector2Int((int)input.x, (int)input.y);

            //showPreview();
        };
        //ctx é contexto
        //aqui ele pega os input e só executa em determinados contextos
        //esse no caso é do ataque da movimentação
        controls.Combat.Move.performed += ctx =>
        controls.Combat.Confirm.performed += ctx =>
        {
            if (!hasMoved && direction != Vector2Int.zero)
            {
                TryMove();
            }
        };
    }

    //se associando a leitura dos controles
    void OnEnable() => controls.Combat.Enable();
    void OnDisable() => controls.Combat.Disable();

    //aqui nós vamos tentar mover o boneco.
    void TryMove()
    {
        //próxima direção desejada com a tentativa
        Vector2Int target = gridUnit.currentGridPos + direction;

        //se a direção desejada existir dentro do grid e for um tile andavel
        if (gridUnit.gridBuilder.tacticalGrid.TryGetValue(target, out var tile) && tile.isWalkable)
        {
            //joga a posição para a posição mundial do tile
            transform.position = tile.worldPos;
            //e bota a posição do grid lógico como a definição também
            gridUnit.currentGridPos = target;
            hasMoved = true;

            //highlight
        }
    }
}
