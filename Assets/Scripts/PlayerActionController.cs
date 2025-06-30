using Unity.VisualScripting;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    //referencia para a classe gridUnit
    private GridUnit gridUnit;
    //direção do input do personagem
    private Vector2Int direction = Vector2Int.zero;
    //flag para ver se ja fez alguma ação ou não.
    private bool hasPlayed = false;

    //meter um highlight de ond vai sair a ação
    [SerializeField] private GameObject HighlightPrefab;
    private GameObject highlightInstance;
    //os controles e tal
    private CombatControls controls;

    //ação executada
    [SerializeField] PlayerCombatMovement testingAction;

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
            //se ja se mexeu, nem começa
            if (hasPlayed) return;
            //está lendo o valor para vector2, pois é um input de cima, baixao , esquerda e direita
            Vector2 input = ctx.ReadValue<Vector2>();
            //aplica na direção desejada
            //Filtra somenete para algo como (1,0) (0,-1) e tal. meio forçado demais, mas ta valendo
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                direction = new Vector2Int((int)Mathf.Sign(input.x), 0);
            else
                direction = new Vector2Int(0, (int)Mathf.Sign(input.y));
            //mostra o highlight insane e se for 00 nao mostra
            if (direction != Vector2Int.zero)
                ShowPreview();
        };
        // esse é quando soltar o botao
        //ctx é o contexto, como sempre
        controls.Combat.Move.canceled += ctx =>
        {
            //bota zero a direção
            direction = Vector2Int.zero;
            //destroy highlights
            if (highlightInstance != null)
                Destroy(highlightInstance);
        };
        //ctx é contexto
        //aqui ele pega os input e só executa em determinados contextos
        //esse no caso é da confirmação da ação
        controls.Combat.Confirm.performed += ctx =>
        {
            if (!hasPlayed && direction != Vector2Int.zero)
            {
                //sempre envia a direção
                testingAction.ExecuteAction(gridUnit.currentGridPos + direction);
                //tira highlight
                ShowPreview();
            }
        };
    }
    //se associando a leitura dos controles
    void OnEnable() => controls.Combat.Enable();
    void OnDisable() => controls.Combat.Disable();
    //mostra um quadrado pra direção selecionada
    void ShowPreview()
    {
        Vector2Int target = gridUnit.currentGridPos + direction;
        if (gridUnit.gridBuilder.tacticalGrid.TryGetValue(target, out var tile))
        {
            if (highlightInstance == null)
            {
                highlightInstance = Instantiate(HighlightPrefab);
            }

            highlightInstance.transform.position = tile.worldPos;
        }
        else if (highlightInstance)
            Destroy(highlightInstance);
    }
}
