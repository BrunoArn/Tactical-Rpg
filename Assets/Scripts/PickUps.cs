using System.Collections;
using UnityEngine;

public class PickUps : MonoBehaviour
{
    [Header("Movement info")]
    [SerializeField] private float pickUpDistance = 5f;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float acelleration = 0.1f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 1.5f;
    [SerializeField] private float popDuration = 1f;

    [SerializeField] SpriteRenderer itemIcon;
    public ItemData item;
    public int quantityDropped;


    Rigidbody2D rigidBody;
    private Vector3 moveDir;
    public GameObject player;


    [Header("Super teste somente")]
    private InventoryManager inventory;
    [SerializeField] GameEvent inventoryUIUpdate;


    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(AnimCurveSpawnRoutine());
    }

    void Update()
    {
        Vector3 playerPos = player.transform.position;

        if (Vector3.Distance(transform.position, playerPos) < pickUpDistance)
        {
            moveDir = (playerPos - transform.position).normalized;
            moveSpeed += acelleration;
        }
        else
        {
            moveDir = Vector3.zero;
            moveSpeed = 0f;
        }
    }

    private void FixedUpdate()
    {
        rigidBody.linearVelocity = moveDir * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.GetComponent<BoxCollider2D>() != null)
        {
            inventory = collision.transform.root.GetComponentInChildren<InventoryManager>();
            bool added = inventory.AddItem(item, quantityDropped);
            if (added)
            {
                inventoryUIUpdate.Raise();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log(" inventary is full");
            }

            Destroy(this.gameObject);
        }
    }

    public void UpdateItem(ItemData item, int quantity)
    {
        this.item = item;
        itemIcon.sprite = item.icon;
        itemIcon.enabled = true;

        quantityDropped = quantity;
    }

    private IEnumerator AnimCurveSpawnRoutine()
    {
        Vector2 startPoint = transform.position;
        float randomX = transform.position.x + Random.Range(-2f, 2f);
        float randomY = transform.position.y + Random.Range(-1f, 1f);

        Vector2 endPoint = new Vector2(randomX, randomY);

        float timePassed = 0f;

        while (timePassed < popDuration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / popDuration;
            float heighT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heighT);

            transform.position = Vector2.Lerp(startPoint, endPoint, linearT) + new Vector2(0f, height);
            yield return null;
        }
    }
}
