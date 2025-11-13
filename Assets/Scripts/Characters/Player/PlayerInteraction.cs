using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteraction;
    private CombatControls controls;

    private Collider2D myCollider;
    [SerializeField] private ContactFilter2D filterContact;
    private Collider2D[] results = new Collider2D[10];

    void Awake()
    {
        controls = new CombatControls();
        myCollider = GetComponent<BoxCollider2D>();

        controls.Exploration.Interact.performed += ContextMenu =>
        {
            OnInteract();
        };
    }

    void OnEnable() => controls.Exploration.Enable();
    void OnDisable() => controls.Exploration.Disable();

    private void OnInteract()
    {
        currentInteraction?.Interact();
        currentInteraction = null;
        RefreshTrigger();
    }

    private void TrySetInteractable(Collider2D collision)
    {
        if(collision == null) return;

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if(interactable == currentInteraction) return; 

            currentInteraction?.ToggleHighlight(false);
            currentInteraction = interactable;
            currentInteraction.ToggleHighlight(true);
        }
    }

    private void RefreshTrigger()
    {
        int count = myCollider.Overlap(filterContact, results);

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (col == null | col.gameObject == null) continue;

            TrySetInteractable(col);
            if (currentInteraction != null)
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        TrySetInteractable(collision);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == currentInteraction)
        {
            currentInteraction.ToggleHighlight(false);
            currentInteraction = null;
        }
    }


}
