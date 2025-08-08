using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [Header("Item info")]
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private ItemData item;
    [SerializeField] int quantity = 1;

    [Header("character info")]
    [SerializeField] Health health;
    public GameObject player;

    private void Start()
    {
        health.OnDeath += DropItems;
    }

    public void DropItems()
    {
        GameObject newItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        newItem.GetComponent<PickUps>().player = player;
        newItem.GetComponent<PickUps>().UpdateItem(item, quantity);
    }
}
