using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class PlayerActionController : MonoBehaviour, ICombatUnit
{
    // reference to GridUnit
    private GridUnit gridUnit;
    // input controls
    private CombatControls controls;
    // input direction
    private Vector2Int direction = Vector2Int.zero;
    // played flag
    private bool hasPlayed = true;

    [SerializeField] LayerMask gridLayer;

    [Header("Highlight")]
    [Space]
    [SerializeField] private GameObject HighlightPrefab;
    [SerializeField] private Sprite tileOn;
    [SerializeField] private Sprite tileOff;
    private GameObject highlightInstance;

    [Header("Action")]
    [SerializeField] MonoBehaviour MoveAction;
    [SerializeField] MonoBehaviour AttackAction;

    // callback to manager
    private Action onTurnEnd;

    [Header("Game Events")]
    [SerializeField] private GameEvent explorationRequest;

    void Awake()
    {
        gridUnit = GetComponent<GridUnit>();
        controls = new CombatControls();

        if (gridUnit == null)
            Debug.LogError($"{nameof(PlayerActionController)} requires a GridUnit component on the same GameObject.");
        if (HighlightPrefab == null)
            Debug.LogWarning($"{nameof(PlayerActionController)}: HighlightPrefab is not assigned.");
    }

    void OnEnable()
    {
        controls.Combat.Direction.performed += OnDirectionPerformed;
        controls.Combat.Direction.canceled += OnDirectionCanceled;
        controls.Combat.Confirm.performed += OnConfirmPerformed;
        controls.Combat.Enable();
    }

    void OnDisable()
    {
        controls.Combat.Direction.performed -= OnDirectionPerformed;
        controls.Combat.Direction.canceled -= OnDirectionCanceled;
        controls.Combat.Confirm.performed -= OnConfirmPerformed;
        controls.Combat.Disable();
    }

    void OnDestroy()
    {
        controls?.Dispose();
    }

    private void OnDirectionPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            direction = new Vector2Int((int)Mathf.Sign(input.x), 0);
        else
            direction = new Vector2Int(0, (int)Mathf.Sign(input.y));

        if (direction != Vector2Int.zero)
            ShowPreview();
    }

    private void OnDirectionCanceled(InputAction.CallbackContext ctx)
    {
        direction = Vector2Int.zero;
        DestroyPreview();
    }

    private void OnConfirmPerformed(InputAction.CallbackContext ctx)
    {
        if (!hasPlayed && direction != Vector2Int.zero)
        {
            TileData targetTile = null;
            if (gridUnit != null && gridUnit.currentTile != null)
                targetTile = gridUnit.currentTile.GetNeighbors(direction);

            IUnitAction candidate = null;

            if (targetTile != null && targetTile.isWalkable && !targetTile.IsOccupied)
                candidate = MoveAction as IUnitAction;
            else if (targetTile != null && targetTile.IsOccupied)
                candidate = AttackAction as IUnitAction;
            else if (gridUnit != null && gridUnit.currentTile != null && gridUnit.currentTile.isBorder && targetTile == null)
            {
                Vector2 dir = new Vector2(direction.x, direction.y);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, gridLayer);
                if (hit.collider == null)
                    explorationRequest?.Raise();
            }

            if (candidate != null)
            {
                candidate.ExecuteAction(targetTile, gridUnit);
                candidate = null;
                BeforeEndTurn();
            }
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
    void ShowPreview()
    {
        TileData targetTile = null;
        if (gridUnit != null && gridUnit.currentTile != null)
            targetTile = gridUnit.currentTile.GetNeighbors(direction);

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

    void UpdatePreviewPrefab()
    {
        if (highlightInstance != null)
        {
            var sr = highlightInstance.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = hasPlayed ? tileOff : tileOn;
        }
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
