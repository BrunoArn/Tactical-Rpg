using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerExplorationMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rigidBody;
    private CombatControls controls;
    private InputAction moveAction;

    private Vector2 inputVector;


    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        controls = new CombatControls();
        moveAction = controls.Exploration.Move;
    }

    void OnEnable()
    {
        controls.Exploration.Enable();
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
    }

    void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        controls.Exploration.Disable();
        rigidBody.linearVelocity = Vector2.zero;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        rigidBody.linearVelocity = inputVector * moveSpeed;
    }
}
