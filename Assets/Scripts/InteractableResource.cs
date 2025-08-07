using UnityEngine;

public class InteractableResource : MonoBehaviour
{
    [SerializeField] SpriteRenderer actionIndicator;
    [SerializeField] ItemData droppedItem;
    [SerializeField] int quantityDropped;

    [Header("Super teste somente")]
    [SerializeField] Inventory inventory;
    [SerializeField] InventoryUi inventoruUI;
    void OnTriggerEnter2D(Collider2D collision)
    {
        actionIndicator.enabled = true;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool added = inventory.AddItem(droppedItem, quantityDropped);
            if (added)
            {
                Destroy(gameObject);
                inventoruUI.Redraw();
            }
            else
                Debug.Log(" inventary is full");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        actionIndicator.enabled = false;
    }
}
