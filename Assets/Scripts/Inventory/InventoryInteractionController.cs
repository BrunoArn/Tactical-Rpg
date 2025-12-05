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


    private InputAction currentNavigateAction;
    private System.Action<InputAction.CallbackContext> currentHandler;
    private int selectedIndex;
    private InventorySlotUi highlightedSlotUi; // para usar o item selecionado

    private CombatControls controls;

    void OnEnable()
    {
        controls = new CombatControls();
        controls.Ui.Interact.performed += UseSelectedItem;
        SetNavigatorTarget();
        controls.Ui.Enable();
    }

    void OnDisable()
    {
        controls.Ui.Interact.performed -= UseSelectedItem;
        UnsubscribeNavigate();
        controls.Ui.Disable();
        controls.Dispose();
    }

    void OnDestroy()
    {
        controls?.Dispose();
    }

    void Start()
    {
        TryInitHighlight();
    }

    public void ChangeState()
    {
        SetNavigatorTarget();
        TryInitHighlight();
    }

    private void SetNavigatorTarget()
    {
        UnsubscribeNavigate();
        switch (gameStateVariable.CurrentState)
        {
            case GameState.Pause:
                currentNavigateAction = controls.Ui.Navigate;
                currentHandler = OnNavigatePause;
                break;
            default:
                currentNavigateAction = controls.Ui.QuickBarNavigate;
                currentHandler = OnNavigateQuickBarOnly;
                break;
        }

        if (currentNavigateAction != null && currentHandler != null)
            currentNavigateAction.performed += currentHandler;
    }

    private void UnsubscribeNavigate()
    {
        if (currentNavigateAction != null && currentHandler != null)
            currentNavigateAction.performed -= currentHandler;
    }


    private void OnNavigatePause(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        var col = selectedIndex % backpackColumns;
        var row = selectedIndex / backpackColumns;
        var newIndex = selectedIndex;

        if(input.x > 0.5f && col < backpackColumns -1) newIndex = selectedIndex + 1;
        else if (input.x < -0.5f && col > 0) newIndex = selectedIndex - 1;
        else if (input.y > 0.5f &&  selectedIndex - backpackColumns >= 0) newIndex = selectedIndex - backpackColumns;
        else if (input.y < -0.5f && selectedIndex + backpackColumns < backpackUi.slotsUI.Count) newIndex = selectedIndex + backpackColumns;

        if(newIndex != selectedIndex)
        {
            backpackUi.slotsUI[selectedIndex].SetHighlight(false);
            selectedIndex = newIndex;
            backpackUi.slotsUI[selectedIndex].SetHighlight(true);
            highlightedSlotUi = backpackUi.slotsUI[selectedIndex];
        }
    }

    private void OnNavigateQuickBarOnly(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        var newIndex = selectedIndex;

        if(input.x > 0.5f && selectedIndex < quickbarUi.slotsUI.Count - 1) newIndex = selectedIndex + 1;
        else if (input.x < -0.5f && selectedIndex > 0) newIndex = selectedIndex - 1;

        if(newIndex != selectedIndex)
        {
            quickbarUi.slotsUI[selectedIndex].SetHighlight(false);
            selectedIndex = newIndex;
            quickbarUi.slotsUI[selectedIndex].SetHighlight(true);
            highlightedSlotUi = quickbarUi.slotsUI[selectedIndex];
        }
    }

    private void TryInitHighlight()
    {
        switch (gameStateVariable.CurrentState)
        {
            //force start on backpack
            case GameState.Pause:
                if (backpackUi == null || backpackUi.slotsUI == null || backpackUi.slotsUI.Count == 0) return;
                selectedIndex = Mathf.Clamp(selectedIndex, 0, backpackUi.slotsUI.Count - 1);
                ClearAllHighlights();
                backpackUi.slotsUI[selectedIndex].SetHighlight(true);
                highlightedSlotUi = backpackUi.slotsUI[selectedIndex];
                break;
            // force start on quickbar, in this case only quickbar
            default:
                if (quickbarUi == null || quickbarUi.slotsUI == null || quickbarUi.slotsUI.Count == 0) return;
                selectedIndex = Mathf.Clamp(selectedIndex, 0, quickbarUi.slotsUI.Count - 1);
                ClearAllHighlights();
                quickbarUi.slotsUI[selectedIndex].SetHighlight(true);
                highlightedSlotUi = quickbarUi.slotsUI[selectedIndex];
                break;
        }
    }
    // clear all highlights from both UIs
    private void ClearAllHighlights()
    {
        for (int i = 0; i < backpackUi.slotsUI.Count; i++)
        {
            backpackUi.slotsUI[i].SetHighlight(false);
        }
        for (int i = 0; i < quickbarUi.slotsUI.Count; i++)
        {
            quickbarUi.slotsUI[i].SetHighlight(false);
        }
    }

    private void UseSelectedItem(InputAction.CallbackContext context)
    {
        UseSelectedItemInternal();
    }

    public void UseSelectedItemInternal()
    {
        if (highlightedSlotUi == null) return;

        var slot = highlightedSlotUi.slotData;
        if (slot == null || slot.item == null) return;

        switch (slot.item.itemType)
        {
            case ItemType.Consumable:
                // Apply consumable effect here
                Debug.Log($"Using consumable: {slot.item.itemName}");
                slot.quantity--;
                if (slot.quantity <= 0)
                {
                    highlightedSlotUi.Clear();
                }
                break;
            case ItemType.Equipment:
                // Equip item here
                Debug.Log($"Equipping item: {slot.item.itemName}");
                if (equipment != null || InventoryManager != null)
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
