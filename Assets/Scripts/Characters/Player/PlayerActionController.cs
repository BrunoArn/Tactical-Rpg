using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerActionController : MonoBehaviour, ICombatUnit
{
    
    //referencia para a classe gridUnit
    private GridUnit gridUnit;
    //os controles e tal
    private CombatControls controls;
    //direção do input do personagem
    private Vector2Int direction = Vector2Int.zero;
    //flag para ver se ja fez alguma ação ou não.
    private bool hasPlayed = true;

    [Header("Highlight")]
    [Space]
    //meter um highlight de ond vai sair a ação
    [SerializeField] private GameObject HighlightPrefab;
    [SerializeField] private Sprite tileOn;
    [SerializeField] private Sprite tileOff;
    private GameObject highlightInstance;

    [Header("Action")]
    //ação executada, ta hardcodedzada por enquantos
    [SerializeField] MonoBehaviour testingAction;
    private IUnitAction action;

    //callback to manager
    private System.Action onTurnEnd;

    #region Setup

    void Awake()
    {
        //pega o componente do grid Unit e pega os controles
        gridUnit = GetComponent<GridUnit>();
        controls = new CombatControls();
        action = testingAction as IUnitAction;

        //agora vem os inputs

        //ctx é contexto
        //aqui ele pega os input e só executa em determinados contextos
        //esse no caso é da direção da ação
        controls.Combat.Direction.performed += ctx =>
        {
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
        controls.Combat.Direction.canceled += ctx =>
        {
            //bota zero a direção
            direction = Vector2Int.zero;
            //destroy highlights
            DestroyPreview();
        };
        //ctx é contexto
        //aqui ele pega os input e só executa em determinados contextos
        //esse no caso é da confirmação da ação
        controls.Combat.Confirm.performed += ctx =>
        {
            if (!hasPlayed && direction != Vector2Int.zero)
            {
                //sempre envia a direção
                action.ExecuteAction(gridUnit.currentGridPos + direction);
                //nao mexe mais
                hasPlayed = true;
                ShowPreview();
                EndTurn();
            }
        };
    }

    //se associando a leitura dos controles
    void OnEnable() => controls.Combat.Enable();
    void OnDisable() => controls.Combat.Disable();
    #endregion

    #region ICombatUnit Interface


    //starta o turno do boneco, libera ele pra açao
    //recebe do manager a info que é a vez dele
    public void StartTurn(Action onTurnEndCallBack)
    {
        onTurnEnd = onTurnEndCallBack;
        hasPlayed = false; // reseta a flag
        //volta a cor da UI
        UpdatePreviewPrefab();

    }
    //termina o turno dele por aqui e por la também
    public void EndTurn()
    {
        onTurnEnd?.Invoke(); // manda pro manager que ta tudo bem
    }

    #endregion


    #region Preview
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

            //checar qual é
            highlightInstance.transform.position = tile.worldPos;
            UpdatePreviewPrefab();
        }
        else if (highlightInstance)
            Destroy(highlightInstance);
    }

    //update o visual do prefa para ficar parecido com se pode ou nao mexer
    void UpdatePreviewPrefab()
    {
        if (highlightInstance != null)
        {
            //ja jogou, troca pro trash
            if (hasPlayed) highlightInstance.GetComponent<SpriteRenderer>().sprite = tileOff;
            //nao jogou fica vermelho
            else highlightInstance.GetComponent<SpriteRenderer>().sprite = tileOn;
        }
        else
        {
            //ja jogou, troca pro trash
            if (hasPlayed) HighlightPrefab.GetComponent<SpriteRenderer>().sprite = tileOff;
            //nao jogou fica vermelho
            else HighlightPrefab.GetComponent<SpriteRenderer>().sprite = tileOn;
        }


    }

    void DestroyPreview()
    {
        if (highlightInstance != null)
            Destroy(highlightInstance);
    }
    #endregion
}
