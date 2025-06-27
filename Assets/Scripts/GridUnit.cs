using UnityEngine;

public class GridUnit : MonoBehaviour
{
    // referencia ao grid... vamos ver como pegar essa informação melhor.
    // por enquanto só quer saber do dicionário
    public TacticalGridBuilder gridBuilder;

    // posição lógica dentro do grid
    public Vector2Int currentGridPos;

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

            if(dist < closestDist)
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
}
