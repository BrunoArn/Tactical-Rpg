using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileData
{
   //normal data
   public Vector2Int gridPos;
   public Vector3 worldPos;
   public bool isWalkable;

   //Flow-Field data
   public int distanceToHero = int.MaxValue;
   public Vector2Int preferredDirection = Vector2Int.zero;

   //neighbors
   public List<TileData> neighbors = new();

   //ocuupied info
   private GridUnit _occupyingUnit;
   public GridUnit OccupyingUnit
   {
      get
      {
         return _occupyingUnit;
      }
      set
      {
         _occupyingUnit = value;
         isWalkable = false;
      }
   }

   public bool IsOccupied => _occupyingUnit != null;

   public TileData(Vector2Int gridPos, Vector3 worldPos, bool isWalkable)
   {
      this.gridPos = gridPos;
      this.worldPos = worldPos;
      this.isWalkable = isWalkable;
   }

   public void AssignNeighbors(Dictionary<Vector2Int, TileData> grid)
   {
      Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
      foreach (var dir in directions)
      {
         var neighborPos = gridPos + dir;
         if (grid.TryGetValue(neighborPos, out var neighbor))
         {
            //Debug.Log(neighbor.gridPos);
            neighbors.Add(neighbor);
         }
      }
   }

   public TileData GetNeighbors(Vector2Int direction)
   {
      foreach (TileData tile in neighbors)
      {
         if (tile.gridPos == this.gridPos + direction)
         {
            return tile;
         }
      }
      return null;
   }

   public void ClearTile()
   {
      isWalkable = true;
      _occupyingUnit = null;
   }
}
