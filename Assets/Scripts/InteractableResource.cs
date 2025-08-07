using UnityEngine;

public class InteractableResource : MonoBehaviour, IInteractable
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

    void OnTriggerExit2D(Collider2D collision)
    {
        actionIndicator.enabled = false;
    }

    public void Interact()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool added = inventory.AddItem(droppedItem, quantityDropped);
            if (added)
            {
                inventoruUI.Redraw();
                Destroy(gameObject);
            }
            else
                Debug.Log(" inventary is full");
        }
    }
}
