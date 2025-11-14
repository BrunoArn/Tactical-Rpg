using UnityEngine;
using System;

public class GridObstacle : MonoBehaviour
{
    // é injetado pelo próprio combatManager;
    public TileData currentTile;

    public Health health;

    public event Action<GridObstacle> OnObstacleDestruction;
}
