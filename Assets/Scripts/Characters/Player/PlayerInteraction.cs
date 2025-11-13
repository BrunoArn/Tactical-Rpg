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
        myCollider = GetComponent<Collider2D>();
    }

    private void OnInteractPerformed(InputAction.CallbackContext _) => OnInteract();

    void OnEnable()
    {
        controls.Exploration.Interact.performed += OnInteractPerformed;
        controls.Exploration.Enable();
    }

    void OnDisable()
    {
        controls.Exploration.Interact.performed -= OnInteractPerformed;
        controls.Exploration.Disable();
    }

    private void OnInteract()
    {
        currentInteraction?.Interact();
        currentInteraction = null;
        RefreshTrigger();
    }

    private void TrySetInteractable(Collider2D collision)
    {
        if (collision == null) return;

        var interactable = collision.GetComponent<IInteractable>();
        if (interactable == null) return;

        if (interactable == currentInteraction) return;

        currentInteraction?.ToggleHighlight(false);
        currentInteraction = interactable;
        currentInteraction.ToggleHighlight(true);
    }

    private void RefreshTrigger()
    {
        int count = myCollider.Overlap(filterContact, results);

        Collider2D nearest = null;
        float nearestSqr = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (col == null || col.gameObject == null) continue;

            var interactable = col.GetComponent<IInteractable>();
            if (interactable == null) continue;

            var dir = (col.transform.position - transform.position);
            float sqr = dir.sqrMagnitude;
            if (sqr < nearestSqr)
            {
                nearestSqr = sqr;
                nearest = col;
            }
        }

        if (nearest != null)
        {
            TrySetInteractable(nearest);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        TrySetInteractable(collision);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteraction)
        {
            currentInteraction.ToggleHighlight(false);
            currentInteraction = null;
            RefreshTrigger();
        }
    }


}
