using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TacticalGridBuilder : MonoBehaviour
{
    [Header("TileMap info")]
    [Space]

    [Tooltip("only tiles in this map will be generated")]
    [SerializeField] Tilemap tacticalTilemap;

    [Tooltip("grid total size")]
    [SerializeField] Vector2Int gridSize = new(10, 10);

    [Tooltip("where the grid will bet set up")]
    //change origin later on with enemy location or player.
    [SerializeField] Vector3Int gridOrigin = new(0, 0, 0);

    [Header("Highlight")]
    [Space]

    [Tooltip("Highlight color")]
    [SerializeField] Color debugColor = new Color(0, 1, 0, 0.25f);
    //deafault unity cellsize
    private float cellSize = 1f;

    //the actual logic tilemap;
    public Dictionary<Vector2Int, TileData> tacticalGrid = new();

    public void StartGrid()
    {
        GenerateTacticalGrid();
        AssignTilesNeighors();
    }

    //gera o grid lógico
    private void GenerateTacticalGrid()
    {
        //reseta o grid antigo
        tacticalGrid.Clear();

        // calcula a metade da altura e largura pra centralizar o grid
        int halfWidth = gridSize.x / 2;
        int halfHeight = gridSize.y / 2;

        //vai em cada cell referente a -half a +half, garantindo o centro
        for (int x = -halfWidth; x < gridSize.x - halfWidth; x++)
        {
            for (int y = -halfHeight; y < gridSize.y - halfHeight; y++)
            {
                //converte a posição local e joga pra cell position
                Vector3Int cell = new Vector3Int(gridOrigin.x + x, gridOrigin.y + y, 0);
                //pega a celular no tilemap de sprites, se nao tiver, retornar null;
                TileBase tile = tacticalTilemap.GetTile(cell);

                //se existir o tile..
                if (tile != null)
                {
                    //pega a posição do mundo dessa cell, centralizada no tile
                    Vector3 worldPos = tacticalTilemap.CellToWorld(cell) + new Vector3(cellSize / 2, cellSize / 2, 0);
                    //pega a posição no grid lógico
                    Vector2Int gridKey = new Vector2Int(x, y);
                    //adiciona no dicionário- e bota que é walkable de graça ja.
                    tacticalGrid[gridKey] = new TileData(gridKey, worldPos, true);
                }
            }
        }
    }

    private void AssignTilesNeighors()
    {
        foreach (TileData tile in tacticalGrid.Values)
        {
            tile.AssignNeighbors(tacticalGrid);
        }
    }

    // give me the grind bounderies
    public Bounds GetGridBounds()
    {
        Vector3 size = new Vector3(gridSize.x, gridSize.y, 1);
        Vector3 center = gridOrigin;
        return new Bounds(center, size);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = debugColor;
        foreach (var tile in tacticalGrid.Values)
        {
            Gizmos.DrawCube(tile.worldPos, Vector3.one * cellSize * 0.95f);
        }
    }


    //printa a porra do grid todo
    [ContextMenu("Print Tactical Grid")]
    public void PrintTacticalGrid()
    {
        Debug.Log("==== Tactical Grid Contents ====");
        foreach (var kvp in tacticalGrid)
        {
            Vector2Int key = kvp.Key;
            TileData tile = kvp.Value;
            Debug.Log($"GridPos: {key} | WorldPos: {tile.worldPos} | Walkable: {tile.isWalkable}");
        }
    }


}
