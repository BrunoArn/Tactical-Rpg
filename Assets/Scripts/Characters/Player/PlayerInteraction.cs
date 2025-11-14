using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Component that manages player interactions with nearby <c>IInteractable</c> objects.
/// </summary>
/// <remarks>
/// The component finds interactable objects within the player's collider
/// using a <see cref="ContactFilter2D"/> and selects the nearest one to
/// highlight and interact with. Interaction is triggered through the
/// generated <c>CombatControls</c> input action map.
/// </remarks>
public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteraction;
    private CombatControls controls;

    [SerializeField] private Collider2D myCollider;
    [SerializeField] private ContactFilter2D filterContact;
    private Collider2D[] results = new Collider2D[10];

    void Awake()
    {
        controls = new CombatControls();

        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Adapter used to subscribe/unsubscribe the input system callback.
    /// Calls <see cref="OnInteract"/> when the input action is performed.
    /// </summary>
    /// <param name="_">Input callback context (unused).</param>
    private void OnInteractPerformed(InputAction.CallbackContext _) => OnInteract();

    void OnEnable()
    {
        controls.Exploration.Interact.performed += OnInteractPerformed;
        controls.Exploration.Enable();
        myCollider.enabled = true;
    }

    void OnDisable()
    {
        controls.Exploration.Interact.performed -= OnInteractPerformed;
        controls.Exploration.Disable();
        myCollider.enabled = false;
    }

    /// <summary>
    /// Perform the current interaction (if any), then clear the selection
    /// and refresh overlapping interactables.
    /// </summary>
    private void OnInteract()
    {
        currentInteraction?.Interact(transform.root.gameObject);
        currentInteraction = null;
        RefreshTrigger();
    }

    /// <summary>
    /// Attempts to set the supplied collider's <c>IInteractable</c> as the
    /// current interaction and toggles highlighting accordingly.
    /// </summary>
    /// <param name="collision">Collider to inspect for an <c>IInteractable</c>.</param>
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

    /// <summary>
    /// Scans overlapping colliders using <see cref="filterContact"/>,
    /// finds the nearest <c>IInteractable</c>, and selects it.
    /// </summary>
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

            var dir = col.transform.position - transform.position;
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

    /// <summary>
    /// Called by Unity when a <c>Collider2D</c> enters this object's trigger.
    /// Attempts to set the collider as the current interactable.
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        TrySetInteractable(collision);
    }

    /// <summary>
    /// Called by Unity when a <c>Collider2D</c> exits this object's trigger.
    /// Clears highlight if the collider was the current interactable and
    /// refreshes to find a new nearest interactable.
    /// </summary>
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
