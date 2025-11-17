using UnityEngine;
using System;

public class GridObstacle : MonoBehaviour
{
    // é injetado pelo próprio combatManager;
    public TileData currentTile;

    public Health health;

    public event Action<GridObstacle> OnObstacleDestruction;

    void Awake()
    {
        if (health != null)
            health.OnDeath += HandleHealthDeath;
    }

    private void HandleHealthDeath()
    {
        OnObstacleDestruction?.Invoke(this);
        currentTile.ClearTile();
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= HandleHealthDeath;
    }
}
