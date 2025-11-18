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
    [SerializeField] MonoBehaviour RangedAttackAction;
    private bool isRanged = false;

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

        controls.Combat.RangedStance.performed += OnRangedStancePerformed;
        controls.Combat.RangedStance.canceled += OnRangedStanceCanceled;

        controls.Combat.Confirm.performed += OnConfirmPerformed;
        controls.Combat.Enable();
    }

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

    private void OnRangedStancePerformed(InputAction.CallbackContext ctx)
    {
        isRanged = true;
        ShowPreview();
    }

    private void OnRangedStanceCanceled(InputAction.CallbackContext ctx)
    {
        isRanged = false;
        ShowPreview();
    }

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
            //move
            if (targetTile != null && targetTile.isWalkable && !targetTile.IsOccupied)
                candidate = MoveAction as IUnitAction;
            //attack
            else if (targetTile != null && targetTile.IsOccupied)
                candidate = AttackAction as IUnitAction;
            //flee
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

    void UpdatePreviewPrefab()
    {
        if (highlightInstance != null)
        {
            var sr = highlightInstance.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = hasPlayed ? tileOff : tileOn;
        }
        //nao tem highlast Instance
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
