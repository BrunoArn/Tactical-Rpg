using UnityEngine;

public class GridUnit : MonoBehaviour
{
    // referencia ao grid... vamos ver como pegar essa informação melhor.
    // caso precise e nao tenha o combatManager injetado
    public TacticalGridBuilder gridBuilder;
    // é injetado pelo próprio combatManager;
    [HideInInspector] public CombatManager combatManager;

    // posição lógica dentro do grid
    public Vector2Int currentGridPos;
    public UnitStats stats;

    [SerializeField] MonoBehaviour actionController;
    private ICombatUnit action;

    void Awake()
    {
        action = actionController as ICombatUnit;
    }

    public void SnapToClosestTile()
    {
        // posição atual da unidade, pode estar fora do grid
        Vector3 currentPos = transform.position;
        //montar a comparação de distancia
        // começa com infinito para que a primeira seja sempre suave
        float closestDist = Mathf.Infinity;
        Vector2Int closestKey = Vector2Int.zero;

        //verifica todos os tiles do grid e descobre qual o mais perto
        foreach (var tileEntry in gridBuilder.tacticalGrid)
        {
            float dist = Vector3.Distance(currentPos, tileEntry.Value.worldPos);

            if (dist < closestDist)
            {
                closestDist = dist;
                closestKey = tileEntry.Key;
            }
        }
        // se achou um tile, snap
        if (gridBuilder.tacticalGrid.TryGetValue(closestKey, out var tileData))
        {
            transform.position = tileData.worldPos;
            currentGridPos = closestKey;
        }
    }

    public void UpdateGridPosition(Vector2Int newPos)
    {
        if (combatManager == null) return;

        combatManager.unitPosition.Remove(currentGridPos);
        combatManager.unitPosition[newPos] = this;
        currentGridPos = newPos;
        combatManager.UpdateTileWalkability();

    }

    public void StartAction(System.Action onTurnEndCallBack)
    {
        action.BeforeStart(onTurnEndCallBack);
    }
}
