using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Detects the player within a circular radius and issues a combat request.
/// </summary>
/// <remarks>
/// The script syncs a <see cref="CircleCollider2D"/> and a
/// <see cref="LineRenderer"/> to visually represent the detection area.
/// On player entry it writes the enemy position into <c>gridOrigin</c>
/// and raises the configured <c>GameEvent</c> to start combat.
/// </remarks>
public class PlayerDetection : MonoBehaviour
{
    [Header("Combat Request")]
    [SerializeField] GameEvent combatRequest;
    [Header("Detection Settings")]
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] GridOriginVariable gridOrigin;

    private CircleCollider2D circleCollider;
    private LineRenderer lineRenderer;

    // number of segments to draw the circle; exposed so it can be tuned in editor
    [SerializeField, Min(3)] private int segmentCount = 60;
    [SerializeField, Min(0f)] private float lineWidth = 0.05f;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        if (circleCollider == null)
        {
            Debug.LogError($"{nameof(PlayerDetection)} on '{gameObject.name}': missing CircleCollider2D. Add one to this GameObject.");
        }
        else
        {
            // ensure trigger mode and radius are in sync
            circleCollider.isTrigger = true;
            circleCollider.radius = detectionRadius;
        }

        if (lineRenderer == null)
        {
            Debug.LogError($"{nameof(PlayerDetection)} on '{gameObject.name}': missing LineRenderer. Add one to this GameObject.");
        }
        else
        {
            // configure the line renderer for a local-space looping circle
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.positionCount = Mathf.Max(3, segmentCount);
            lineRenderer.widthMultiplier = lineWidth;

            DrawDetectionCircle();
        }
    }

    private void DrawDetectionCircle()
    {
        if (lineRenderer == null) return;

        int segments = Mathf.Max(3, segmentCount);
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Cos(angle) * detectionRadius;
            float y = Mathf.Sin(angle) * detectionRadius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<BoxCollider2D>() != null)
        {
            Vector3 position = transform.position;
            Vector2Int positionInt = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            gridOrigin.SetValue(positionInt);
            if (combatRequest != null)
                combatRequest.Raise();
            else
                Debug.LogWarning($"{nameof(PlayerDetection)} on '{gameObject.name}': combatRequest GameEvent is not assigned.");
        }
    }

    /// <summary>
    /// Called by the Unity editor when properties change in the Inspector.
    /// Keeps the CircleCollider2D and LineRenderer synchronized with the
    /// serialized fields so changes are visible immediately in the editor.
    /// </summary>
    void OnValidate()
    {
        // Keep collider and line renderer in sync when values change in the inspector
        if (circleCollider == null)
            circleCollider = GetComponent<CircleCollider2D>();
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (circleCollider != null)
        {
            circleCollider.isTrigger = true;
            circleCollider.radius = detectionRadius;
        }

        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.positionCount = Mathf.Max(3, segmentCount);
            lineRenderer.widthMultiplier = Mathf.Max(0f, lineWidth);
            DrawDetectionCircle();
        }
    }

}
