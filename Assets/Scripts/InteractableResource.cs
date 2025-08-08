using UnityEngine;

public class InteractableResource : MonoBehaviour, IInteractable
{
    [SerializeField] SpriteRenderer highLight;
    [SerializeField] GameObject itemDropPrefab;
    [SerializeField] ItemData droppedItem;
    [SerializeField] int quantityDropped;

    private GameObject player;

    public void Interact()
    {
        GameObject newItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        newItem.GetComponent<PickUps>().player = player;
        newItem.GetComponent<PickUps>().UpdateItem(droppedItem, quantityDropped);

        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
        }
    }

    public void ToggleHighlight(bool highlightMode)
    {
        highLight.enabled = highlightMode;
    }
}
