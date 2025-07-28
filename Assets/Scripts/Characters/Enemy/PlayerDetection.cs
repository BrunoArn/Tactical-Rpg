using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] GameEvent combatRequest;

    [SerializeField] float detectionRadius = 10f;
    private CircleCollider2D circleCollider;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        circleCollider.radius = detectionRadius;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            combatRequest.Raise();
        }
    }
}
