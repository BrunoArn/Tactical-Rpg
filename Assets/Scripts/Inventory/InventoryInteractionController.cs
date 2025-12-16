using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInteractionController : MonoBehaviour
{
    //rever referencias
    [Header("UI References")]
    [SerializeField] private InventoryUi backpackUi;
    [SerializeField] private int backpackColumns = 6;
    [Space]
    [SerializeField] private InventoryUi quickbarUi;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Events and states")]
    [SerializeField] private GameEvent redrawEvent;
    [SerializeField] private GameStateVariable gameStateVariable;

    [Header("Interaction References")]
    [SerializeField] private Equipment equipment;
    [SerializeField] private InventoryDragController dragController;
    private bool isDragging = false;

    private NavigationUI backpackNavigator;
    private NavigationUI quickbarNavigator;
    private NavigationUI currentNavigator;

    private InputAction currentNavigateAction;
    private CombatControls controls;

    private void Awake()
    {
        backpackUi.EnsureSlotsCached();
        foreach (var slot in backpackUi.slotsUI)
        {
            slot.Hovered += SetHoverSlot;
            slot.Clicked += OnSlotCliked;
        }

        quickbarUi.EnsureSlotsCached();
        foreach (var slot in quickbarUi.slotsUI)
        {
            slot.Hovered += SetHoverSlot;
            slot.Clicked += OnSlotCliked;
        }
    }


    void OnEnable()
    {
        controls = new CombatControls();

        backpackNavigator = new NavigationUI(backpackUi.slotsUI, backpackColumns, 0);
        quickbarNavigator = new NavigationUI(quickbarUi.slotsUI, quickbarUi.slotsUI.Count, 0);

        controls.Ui.Interact.performed += UseSelectedItem;

        controls.Ui.GrabItem.performed += GrabHandler;
        controls.Ui.CancelGrab.performed += OnCancel;
        SetActiveNavigator();

        controls.Ui.Enable();
    }

    void OnDisable()
    {
        if (controls == null) return;

        controls.Ui.Interact.performed -= UseSelectedItem;

        controls.Ui.GrabItem.performed -= GrabHandler;
        controls.Ui.CancelGrab.performed -= OnCancel;
        UnsubscribeNavigate();

        controls.Ui.Disable();
        controls.Dispose();
    }

    #region Navigation

    public void SetActiveNavigator()
    {
        UnsubscribeNavigate();
        currentNavigator?.ClearHighlights();
        switch (gameStateVariable.CurrentState)
        {
            case GameState.Pause:
                currentNavigator = backpackNavigator;
                currentNavigateAction = controls.Ui.Navigate;
                break;

            default:
                currentNavigator = quickbarNavigator;
                currentNavigateAction = controls.Ui.QuickBarNavigate;
                break;
        }

        currentNavigator.SetIndex(currentNavigator.CurrentHighlightedSlot != null ? currentNavigator.selectedIndex : 0);
        if (currentNavigateAction != null)
            currentNavigateAction.performed += OnNavigate;
    }

    private void UnsubscribeNavigate()
    {
        if (currentNavigateAction != null)
            currentNavigateAction.performed -= OnNavigate;
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (currentNavigator == null) return;
        var input = context.ReadValue<Vector2>();

        if (gameStateVariable.CurrentState == GameState.Pause)
        {
            if (input.y < -0.5f && currentNavigator == backpackNavigator && IsOnLastBackpackRow())
            {
                JumpToQuickbar();
                if (isDragging) dragController.UpdateGhostToSlot(currentNavigator.CurrentHighlightedSlot);
                return;
            }
            if (input.y > 0.5f && currentNavigator == quickbarNavigator)
            {
                JumpToBackpackLastRow();
                if (isDragging) dragController.UpdateGhostToSlot(currentNavigator.CurrentHighlightedSlot);
                return;
            }
        }

        currentNavigator.Navigate(input);
        
        if (isDragging) dragController.UpdateGhostToSlot(currentNavigator.CurrentHighlightedSlot);
    }

    private NavigationUI GetNavigatorForSlot(InventorySlotUi slot)
    {
        if (backpackUi.slotsUI.Contains(slot)) return backpackNavigator;
        if (quickbarUi.slotsUI.Contains(slot)) return quickbarNavigator;
        return null;
    }

    private void SwitchNavigator(NavigationUI next)
    {
        if (currentNavigator == next) return;
        currentNavigator?.ClearHighlights();
        currentNavigator = next;
    }
    #endregion

    #region Pause navigation Helper

    private bool IsOnLastBackpackRow()
    {
        if (backpackUi.slotsUI.Count == 0) return true;
        var rows = Mathf.CeilToInt(backpackUi.slotsUI.Count / (float)backpackColumns);
        var lastRowStart = (rows - 1) * backpackColumns;
        return backpackNavigator.selectedIndex >= lastRowStart;
    }

    private void JumpToQuickbar()
    {
        if (quickbarUi.slotsUI.Count == 0) return;
        SwitchNavigator(quickbarNavigator);
        quickbarNavigator.SetIndex(0);
    }

    private void JumpToBackpackLastRow()
    {
        if (backpackUi.slotsUI.Count == 0) return;
        var rows = Mathf.CeilToInt(backpackUi.slotsUI.Count / (float)backpackColumns);
        var lastRowStart = Mathf.Max(0, (rows - 1) * backpackColumns);
        SwitchNavigator(backpackNavigator);
        backpackNavigator.SetIndex(lastRowStart);
    }
    #endregion

    #region Interactions

    private void SetHoverSlot(InventorySlotUi slot)
    {
        if (slot == null || currentNavigator == null) return;

        var navigator = GetNavigatorForSlot(slot);
        if (navigator == null) return;
        SwitchNavigator(navigator);

        //find index
        var navigatorList = currentNavigator == backpackNavigator ? backpackUi.slotsUI : quickbarUi.slotsUI;
        var index = navigatorList.IndexOf(slot);
        if (index >= 0) currentNavigator.SetIndex(index);
    }

    //input manager
    private void UseSelectedItem(InputAction.CallbackContext context)
    {
        UseSelectedItemInternal();
    }

    //clicked
    public void OnSlotCliked(InventorySlotUi slot)
    {
        UseSelectedItemInternal();
    }

    private void UseSelectedItemInternal()
    {
        if (currentNavigator.CurrentHighlightedSlot == null) return;

        var slot = currentNavigator.CurrentHighlightedSlot.SlotData;
        if (slot == null || slot.item == null) return;

        switch (slot.item.itemType)
        {
            case ItemType.Consumable:
                // Apply consumable effect here
                Debug.Log($"Using consumable: {slot.item.itemName}");
                slot.quantity--;
                if (slot.quantity <= 0)
                {
                    currentNavigator.CurrentHighlightedSlot.Clear();
                    redrawEvent.Raise();
                }
                break;
            case ItemType.Equipment:
                // Equip item here
                Debug.Log($"Equipping item: {slot.item.itemName}");
                if (equipment != null && inventoryManager != null)
                {
                    var previousItem = equipment.Equip(slot.item as EquipmentItem);
                    inventoryManager.RemoveItem(slot.item, 1);
                    if (previousItem != null)
                    {
                        inventoryManager.AddItem(previousItem, 1);
                    }
                    redrawEvent.Raise();
                }
                break;
            default:
                Debug.Log("Item type not recognized.");
                break;
        }
    }

    private void GrabHandler(InputAction.CallbackContext ctx)
    {
        if (gameStateVariable.CurrentState != GameState.Pause) return;
        
        if (isDragging)
        {
            OnDrop();
            isDragging = false;
        }
        else
        {
            OnGrab();
            isDragging = true;
        }

    }

    private void OnGrab()
    {
        var slot = currentNavigator.CurrentHighlightedSlot;
        if (slot == null) return;
        dragController.BeginDrag(slot);
    }

    private void OnDrop()
    {
        var slot = currentNavigator.CurrentHighlightedSlot;
        if (slot == null) { dragController.CancelDrag(); return; }
        dragController.DropOn(slot);
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        dragController.CancelDrag();
    }
    #endregion
}
