using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player combat input and previews.
///
/// Responsibilities:
/// - Read combat input (direction, confirm, ranged stance)
/// - Compute and display a preview highlight for the selected action (move/attack/ranged)
/// - Delegate execution to configured <see cref="IUnitAction"/> components
///
/// </summary>
public class PlayerActionController : MonoBehaviour, ICombatUnit
{
    // reference to the unit/grid data for this character
    private GridUnit gridUnit;
    // generated input wrapper for combat actions
    private CombatControls controls;
    // current input direction (cardinal Vector2Int)
    private Vector2Int direction = Vector2Int.zero;
    // whether this unit has already acted this turn
    private bool hasPlayed = true;

    [SerializeField] LayerMask gridLayer;

    [Header("Highlight")]
    [Space]
    // prefab used to mark a target tile in the world
    [SerializeField] private GameObject HighlightPrefab;
    // sprite used when the target is actionable (on)
    [SerializeField] private Sprite tileOn;
    // sprite used when preview is disabled / already acted
    [SerializeField] private Sprite tileOff;
    // runtime instance used for the single primary highlight
    private GameObject highlightInstance;

    [Header("Action")]
    // Assignable action components. These are held as MonoBehaviour so you can
    // plug any component that implements IUnitAction or IRangedAction.
    [SerializeField] MonoBehaviour MoveAction;
    [SerializeField] MonoBehaviour AttackAction;
    [SerializeField] MonoBehaviour RangedAttackAction;
    // input-state: is the player currently holding the ranged stance modifier?
    private bool isRanged = false;

    // callback to manager
    private Action onTurnEnd;

    [Header("Game Events")]
    [SerializeField] private GameEvent explorationRequest;

    /// <summary>
    /// Initialize references and create the input wrapper.
    /// </summary>
    void Awake()
    {
        gridUnit = GetComponent<GridUnit>();
        controls = new CombatControls();

        if (gridUnit == null)
            Debug.LogError($"{nameof(PlayerActionController)} requires a GridUnit component on the same GameObject.");
        if (HighlightPrefab == null)
            Debug.LogWarning($"{nameof(PlayerActionController)}: HighlightPrefab is not assigned.");
    }

    /// <summary>
    /// Register input callbacks when the component is enabled.
    /// Subscriptions are removed in <see cref="OnDisable"/> to avoid leaks.
    /// </summary>
    void OnEnable()
    {
        controls.Combat.Direction.performed += OnDirectionPerformed;
        controls.Combat.Direction.canceled += OnDirectionCanceled;

        controls.Combat.RangedStance.performed += OnRangedStancePerformed;
        controls.Combat.RangedStance.canceled += OnRangedStanceCanceled;

        controls.Combat.Confirm.performed += OnConfirmPerformed;
        controls.Combat.Enable();
    }

    /// <summary>
    /// Clean up input callbacks and previews when the component is disabled.
    /// </summary>
    void OnDisable()
    {
        controls.Combat.Direction.performed -= OnDirectionPerformed;
        controls.Combat.Direction.canceled -= OnDirectionCanceled;

        controls.Combat.RangedStance.performed -= OnRangedStancePerformed;
        controls.Combat.RangedStance.canceled -= OnRangedStanceCanceled;

        controls.Combat.Confirm.performed -= OnConfirmPerformed;
        DestroyPreview();
        isRanged = false;
        controls.Combat.Disable();
    }

    void OnDestroy()
    {
        controls?.Dispose();
    }

    /// <summary>
    /// Called when the directional input is performed. Converts a Vector2 into
    /// a cardinal Vector2Int (preferred axis) and updates the preview.
    /// </summary>
    private void OnDirectionPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            direction = new Vector2Int((int)Mathf.Sign(input.x), 0);
        else
            direction = new Vector2Int(0, (int)Mathf.Sign(input.y));

        ShowPreview();
    }

    private void OnDirectionCanceled(InputAction.CallbackContext ctx)
    {
        direction = Vector2Int.zero;
        DestroyPreview();
    }

    /// <summary>
    /// Enter ranged stance (modifier held). Preview behavior changes while true.
    /// </summary>
    private void OnRangedStancePerformed(InputAction.CallbackContext ctx)
    {
        isRanged = true;
        ShowPreview();
    }

    /// <summary>
    /// Exit ranged stance.
    /// </summary>
    private void OnRangedStanceCanceled(InputAction.CallbackContext ctx)
    {
        isRanged = false;
        ShowPreview();
    }

    /// <summary>
    /// Confirm (execute) the currently-previewed action.
    /// Logic flow:
    /// - If not ranged: try move -> melee attack -> exploration exit
    /// - If ranged: ask the assigned ranged action (IRangedAction) for a target and execute it
    /// </summary>
    private void OnConfirmPerformed(InputAction.CallbackContext ctx)
    {
        if (hasPlayed) return;
        if (direction == Vector2Int.zero) return;

        TileData targetTile = null;
        if (gridUnit != null && gridUnit.currentTile != null)
            targetTile = gridUnit.currentTile.GetNeighbors(direction);

        IUnitAction candidate = null;
        if (!isRanged)
        {
            // Move: walkable + not occupied
            if (targetTile != null && targetTile.isWalkable && !targetTile.IsOccupied)
                candidate = MoveAction as IUnitAction;
            // Melee attack: occupied adjacent tile
            else if (targetTile != null && targetTile.IsOccupied)
                candidate = AttackAction as IUnitAction;
            // Flee / exploration: at border and confirm into empty space
            else if (gridUnit != null && gridUnit.currentTile != null && gridUnit.currentTile.isBorder && targetTile == null)
            {
                Vector2 dir = new Vector2(direction.x, direction.y);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, gridLayer);
                if (hit.collider == null)
                    explorationRequest?.Raise();
            }
        }
        else if (isRanged)
        {
            Debug.Log("Ranged Attack Attempt");
            var ranged = RangedAttackAction as IRangedAction;

            if (ranged != null)
            {
                // Ask the action for its canonical target given origin and direction
                targetTile = ranged.FindTarget(gridUnit, direction);
                if (targetTile != null)
                {
                    candidate = RangedAttackAction as IUnitAction;
                }
            }
            else
            {
                Debug.LogWarning("Ranged stance active but no IRangedAction found on assigned actions.");
            }
        }

        if (candidate != null)
        {
            candidate.ExecuteAction(targetTile, gridUnit);
            BeforeEndTurn();
        }
    }

    #region ICombatUnit
    public void BeforeStart(Action onTurnEndCallBack)
    {
        onTurnEnd = onTurnEndCallBack;
        if (gridUnit != null)
        {
            gridUnit.stats.AddMeter(gridUnit.stats.speed);
            if (gridUnit.stats.Meter >= gridUnit.stats.MeterMax)
                StartTurn();
            else
                EndTurn();
        }
    }

    public void StartTurn()
    {
        hasPlayed = false;
        UpdatePreviewPrefab();
    }

    public void BeforeEndTurn()
    {
        if (gridUnit != null)
            gridUnit.stats.AddMeter(-gridUnit.stats.MeterMax);
        EndTurn();
    }

    public void EndTurn()
    {
        if (gridUnit != null)
            gridUnit.stats.AddMeter(0);

        hasPlayed = true;
        onTurnEnd?.Invoke();

        if (controls != null && controls.Combat.Direction.IsPressed())
        {
            UpdatePreviewPrefab();
            ShowPreview();
        }
    }
    #endregion

    #region Preview
    /// <summary>
    /// Compute and display a preview for the current direction and stance.
    /// If ranged, asks the ranged action for its target; otherwise shows the adjacent tile.
    /// </summary>
    void ShowPreview()
    {
        if (direction == Vector2Int.zero) { DestroyPreview(); return; }
        TileData targetTile = null;
        if (gridUnit != null && gridUnit.currentTile != null)
        {
            if (isRanged)
            {
                var ranged = RangedAttackAction as IRangedAction;
                if (ranged != null)
                    targetTile = ranged.FindTarget(gridUnit, direction);
            }
            else
            {
                targetTile = gridUnit.currentTile.GetNeighbors(direction);
            }
        }

        if (targetTile != null)
        {
            if (highlightInstance == null && HighlightPrefab != null)
                highlightInstance = Instantiate(HighlightPrefab);

            if (highlightInstance != null)
            {
                highlightInstance.transform.position = targetTile.worldPos;
                UpdatePreviewPrefab();
            }
        }
        else if (highlightInstance != null)
        {
            Destroy(highlightInstance);
        }
    }

    /// <summary>
    /// Update the preview sprite depending on whether the unit already acted.
    /// The highlight prefab is reused and its sprite swapped between <see cref="tileOn"/> and <see cref="tileOff"/>.
    /// </summary>
    void UpdatePreviewPrefab()
    {
        if (highlightInstance != null)
        {
            var sr = highlightInstance.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = hasPlayed ? tileOff : tileOn;
        }
        // If the prefab is not instantiated we still keep its sprite data in sync
        else if (HighlightPrefab != null)
        {
            var sr = HighlightPrefab.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = hasPlayed ? tileOff : tileOn;
        }
    }

    void DestroyPreview()
    {
        if (highlightInstance != null) Destroy(highlightInstance);
    }
    #endregion
}
