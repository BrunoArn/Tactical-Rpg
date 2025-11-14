using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player movement in exploration mode using the Input System.
/// Moves the <c>Rigidbody2D</c> according to input and preserves analog
/// stick magnitude while clamping maximum speed to <see cref="moveSpeed"/>.
/// </summary>
public class PlayerExplorationMovement : MonoBehaviour
{
    [Min(0f)][SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Rigidbody2D rigidBody;
    private CombatControls controls;
    private InputAction moveAction;

    // Latest cached input vector from the input system callback.
    private Vector2 inputVector;

    void Awake()
    {
        controls = new CombatControls();
        moveAction = controls.Exploration.Move;

        // Ensure we have a Rigidbody2D reference; inspector-assigned wins otherwise
        if (rigidBody == null)
            rigidBody = transform.root.GetComponentInChildren<Rigidbody2D>();
    }

    void OnEnable()
    {
        controls.Exploration.Enable();

        if (moveAction != null)
        {
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
        }
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
        }

        controls.Exploration.Disable();

        // Stop movement immediately when disabling exploration
        inputVector = Vector2.zero;
        if (rigidBody != null)
            rigidBody.linearVelocity = Vector2.zero;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (rigidBody == null) return;

        // Preserve analog magnitude (for gamepad) but clamp keyboard diagonal boosts
        var move = inputVector;
        if (move.sqrMagnitude > 1f)
            move = move.normalized;

        rigidBody.linearVelocity = move * moveSpeed;
    }

    void OnDestroy()
    {
        // Dispose generated input controls if supported
        try { controls?.Dispose(); } catch { }
    }
}
