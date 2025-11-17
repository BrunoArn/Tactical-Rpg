using System.Collections.Generic;
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
    [Header("Collider & Contact Filter")]
    [SerializeField] private Collider2D myCollider;
    [SerializeField] private ContactFilter2D filterContact;
    private Collider2D[] results = new Collider2D[10];
    // currently overlapping interactables (deduped by component instance)
    private readonly HashSet<IInteractable> interactablesInRange = new HashSet<IInteractable>();
    [Header("Movement Recompute")]
    [Tooltip("When the player moves more than this distance, recompute the nearest interactable.")]
    [SerializeField] private float movementRecomputeDistance = 0.1f;
    private Vector3 lastPosition;
    [SerializeField]
    private bool recomputeWhileMoving = true;

    void Awake()
    {
        controls = new CombatControls();

        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (!recomputeWhileMoving) return;

        // recompute the nearest interactable when the player has moved beyond the threshold
        if ((transform.position - lastPosition).sqrMagnitude > movementRecomputeDistance)
        {
            lastPosition = transform.position;
            RecomputeNearest();
        }
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
        //RefreshTrigger();
        RecomputeNearest();
    }

    /// <summary>
    /// Attempts to set the supplied collider's <c>IInteractable</c> as the
    /// current interaction and toggles highlighting accordingly.
    /// </summary>
    /// <param name="collision">Collider to inspect for an <c>IInteractable</c>.</param>
    // Adds an interactable candidate and recomputes the nearest
    private void AddCandidate(IInteractable interactable)
    {
        if (interactable == null) return;
        if (interactablesInRange.Add(interactable))
            RecomputeNearest();
    }

    // Removes a candidate and recomputes nearest (clears highlight if it was current)
    private void RemoveCandidate(IInteractable interactable)
    {
        if (interactable == null) return;
        if (interactablesInRange.Remove(interactable))
        {
            if (interactable == currentInteraction)
            {
                currentInteraction.ToggleHighlight(false);
                currentInteraction = null;
            }
            RecomputeNearest();
        }
    }

    // Choose the nearest interactable from the candidate set and toggle highlights
    private void RecomputeNearest()
    {
        IInteractable nearest = null;
        float nearestSqr = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var it in interactablesInRange)
        {
            if (it == null) continue;
            var comp = it as Component;
            if (comp == null) continue;

            float sqr = (comp.transform.position - myPos).sqrMagnitude;
            if (sqr < nearestSqr)
            {
                nearestSqr = sqr;
                nearest = it;
            }
        }

        if (nearest != currentInteraction)
        {
            currentInteraction?.ToggleHighlight(false);
            currentInteraction = nearest;
            currentInteraction?.ToggleHighlight(true);
        }
    }

    /// <summary>
    /// Called by Unity when a <c>Collider2D</c> enters this object's trigger.
    /// Attempts to set the collider as the current interactable.
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        var interactable = collision.GetComponentInParent<IInteractable>();
        AddCandidate(interactable);
    }

    /// <summary>
    /// Called by Unity when a <c>Collider2D</c> exits this object's trigger.
    /// Clears highlight if the collider was the current interactable and
    /// refreshes to find a new nearest interactable.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        RemoveCandidate(interactable);
    }
}