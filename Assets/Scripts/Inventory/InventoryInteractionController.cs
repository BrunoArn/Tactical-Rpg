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
    [SerializeField] private InventoryManager InventoryManager;

    [Header("Events and states")]
    [SerializeField] private GameEvent redrawEvent;
    [SerializeField] private GameStateVariable gameStateVariable;

    [Header("Interaction References")]
    [SerializeField] private Equipment equipment;

    private NavigationUI backpackNavigator;
    private NavigationUI quickbarNavigator;
    private NavigationUI currentNavigator;

    private InputAction currentNavigateAction;

    private CombatControls controls;

    private void Awake()
    {
        backpackUi.EnsureSlotsCached();
        foreach (var slot in backpackUi.slotsUI) slot.Hovered += SetHoverSlot;

        quickbarUi.EnsureSlotsCached();
        //foreach (var slot in quickbarUi.slotsUI) slot.Hovered += SetHoverSlot;
    }

    void OnEnable()
    {
        controls = new CombatControls();

        backpackNavigator = new NavigationUI(backpackUi.slotsUI, backpackColumns, 0);
        quickbarNavigator = new NavigationUI(quickbarUi.slotsUI, quickbarUi.slotsUI.Count, 0);

        controls.Ui.Interact.performed += UseSelectedItem;
        SetActiveNavigator();

        controls.Ui.Enable();
    }

    void OnDisable()
    {
        controls.Ui.Interact.performed -= UseSelectedItem;
        UnsubscribeNavigate();

        controls.Ui.Disable();
        controls.Dispose();
    }

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
        currentNavigator.Navigate(input);
    }

    private void SetHoverSlot(InventorySlotUi slot)
    {
        if (slot == null || currentNavigator == null) return;

        //find index
        var navigatorList = currentNavigator == backpackNavigator ? backpackUi.slotsUI : quickbarUi.slotsUI;
        var index = navigatorList.IndexOf(slot);
        if (index >= 0) currentNavigator.SetIndex(index);
    }


    private void UseSelectedItem(InputAction.CallbackContext context)
    {
        UseSelectedItemInternal();
    }

    public void UseSelectedItemInternal()
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
                if (equipment != null && InventoryManager != null)
                {
                    var previousItem = equipment.Equip(slot.item as EquipmentItem);
                    InventoryManager.RemoveItem(slot.item, 1);
                    if (previousItem != null)
                    {
                        InventoryManager.AddItem(previousItem, 1);
                    }
                    redrawEvent.Raise();
                }
                break;
            default:
                Debug.Log("Item type not recognized.");
                break;
        }
    }

}
