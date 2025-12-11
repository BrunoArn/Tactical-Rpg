using System;
using UnityEngine.UI;
using UnityEngine;

public class EquipmentUi : MonoBehaviour
{
    [System.Serializable]
    public struct EquipmentUiSlot
    {
        public EquipmentSlotType slotType;
        public Image icon;
    }

    [SerializeField] Equipment equipment;
    [SerializeField] InventoryManager inventory;
    [SerializeField] EquipmentUiSlot[] uiSlots;
    [SerializeField] GameEvent uiRefresh;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (equipment == null || uiSlots == null) return;

        foreach (var uiSlot in uiSlots)
        {
            var equippedItem = equipment.GetEquippedItem(uiSlot.slotType);
            if (equippedItem != null)
            {
                Debug.Log("Equipped item found: " + equippedItem.name);
                uiSlot.icon.sprite = equippedItem.icon;
                uiSlot.icon.enabled = true;
            }
            else
            {
                uiSlot.icon.sprite = null;
                uiSlot.icon.enabled = false;
            }
        }
    }

    public void RemoveEquipment(EquipmentSlotType equipmentType)
    {
        if (equipment == null || inventory == null) return;

        var removedItem = equipment.Unequip(equipmentType);
        if(removedItem == null) return;

        if (!inventory.AddItem(removedItem, 1)) equipment.Equip(removedItem);
        
        uiRefresh.Raise();
    }
}
