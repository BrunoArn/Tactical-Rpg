using UnityEngine;

/*
 InteractableResource
 --------------------
 A simple interactable that spawns an item when the player interacts with it.

 - Assign a highlight <see cref="SpriteRenderer"/> to visually indicate
   when the object is targetable.
 - Assign an <c>itemDropPrefab</c> that contains a <c>PickUps</c> component
   so the spawned object can be picked up by the player.
 - <c>droppedItem</c> and <c>quantityDropped</c> define the item content.
*/

/// <summary>
/// Simple interactable resource that spawns a pickup when interacted with.
/// </summary>
public class InteractableResource : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer highLight;
    [SerializeField] private PickUpSpawner pickupSpawner;

    /// <summary>
    /// Called by other systems (e.g. the player's interaction controller) to
    /// perform the interaction. Spawns the configured <c>itemDropPrefab</c>
    /// and initializes its <c>PickUps</c> component, then destroys this
    /// resource GameObject.
    /// </summary>
    public void Interact(GameObject player)
    {
        pickupSpawner.player = player;
        pickupSpawner.DropItems();
        Destroy(gameObject);
    }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Toggle the highlight sprite renderer used to indicate this object is
    /// currently selectable/targeted. Safely handles a missing renderer.
    /// </summary>
    /// <param name="highlightMode">True to enable highlight, false to disable.</param>
    public void ToggleHighlight(bool highlightMode)
    {
        if (highLight != null)
            highLight.enabled = highlightMode;
    }
}
