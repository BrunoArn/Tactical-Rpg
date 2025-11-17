using System.Collections.Generic;
using UnityEngine;

public interface IRangedAction
{
    int MaxRange { get; }
    // Returns the primary target tile (first occupied, or null)
    TileData FindTarget(GridUnit origin, Vector2Int dir);
    IEnumerable<TileData> GetPreviewTiles(GridUnit origin, Vector2Int dir);
}
