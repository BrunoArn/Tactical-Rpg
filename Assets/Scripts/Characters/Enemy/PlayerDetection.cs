using System;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header("Combat Request")]
    [SerializeField] GameEvent combatRequest;
    [Header("Detection Settings")]
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] GridOriginVariable gridOrigin;

    private CircleCollider2D circleCollider;
    private LineRenderer lineRenderer;

    // numeros de segmentos pra fazer o ciruclo
    private const int segmentCount = 60;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        //configura o collider pra garantir ne
        circleCollider.isTrigger = true;
        circleCollider.radius = detectionRadius;

        //configura o line render
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segmentCount;
        lineRenderer.widthMultiplier = 0.05f;

        DrawDetectionCircle();
    }

    private void DrawDetectionCircle()
    {
        float angleStep = 360f / segmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Cos(angle) * detectionRadius;
            float y = Mathf.Sin(angle) * detectionRadius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector3 position = transform.position;
            Vector2Int positionInt = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            gridOrigin.SetValue(positionInt);
            combatRequest?.Raise();
        }
    }
}
