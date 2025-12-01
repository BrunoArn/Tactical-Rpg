using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInteractionController : MonoBehaviour
{
    [SerializeField] private BackpackInputUi backpackInputUi;
    [SerializeField] private QuickbarInputUi quickbarInputUi;
    [SerializeField] private InventoryManager InventoryManager;
    [SerializeField] private Equipment equipment;
    [SerializeField] private bool isQuickbar;

    private CombatControls controls;

    void OnEnable()
    {
        controls = new CombatControls();
        controls.Ui.Interact.performed += UseSelectedItem;
        controls.Ui.Enable();
    }

    void OnDisable()
    {
        controls.Ui.Interact.performed -= UseSelectedItem;
        controls.Ui.Disable();
        controls.Dispose();
    }

    void OnDestroy()
    {
        controls?.Dispose();
    }

    public void SetQuickbar(bool value)
    {
        isQuickbar = value;
    }

    public void UseSelectedItem(InputAction.CallbackContext context)
    {
        InventorySlotUi slotui = null;
        if (isQuickbar)
        {
            slotui = quickbarInputUi.CurrentSlotUi;
        }
        else
        {
            slotui = backpackInputUi.CurrentSlotUi;
        }

        var slot = slotui.slotData;
        if (slot == null || slot.item == null) return;

        switch (slot.item.itemType)
        {
            case ItemType.Consumable:
                // Apply consumable effect here
                Debug.Log($"Using consumable: {slot.item.itemName}");
                slot.quantity--;
                if (slot.quantity <= 0)
                {
                    slotui.Clear();
                }
                break;
            case ItemType.Equipment:
                // Equip item here
                Debug.Log($"Equipping item: {slot.item.itemName}");
                if (equipment != null || InventoryManager != null)
                {
                    equipment.Equip(slot.item as EquipmentItem);
                    InventoryManager.RemoveItem(slot.item, 1);
                }
                break;
            default:
                Debug.Log("Item type not recognized.");
                break;
        }
    }

}
