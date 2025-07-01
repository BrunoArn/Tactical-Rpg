using UnityEngine;

public class TileData
{
   public Vector2Int gridPos;
   public Vector3 worldPos;
   public bool isWalkable;

   public TileData(Vector2Int gridPos, Vector3 worldPos, bool isWalkable)
   {
        this.gridPos = gridPos;
        this.worldPos = worldPos;
        this.isWalkable = isWalkable;
   }
}
