using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteraction;
    private CombatControls controls;

    void Awake()
    {
        controls = new CombatControls();

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
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            currentInteraction = interactable;
            currentInteraction.ToggleHighlight(true);
            
        }
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
