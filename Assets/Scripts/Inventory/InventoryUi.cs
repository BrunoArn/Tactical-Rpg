using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUi : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    private List<InventorySlotUi> slotsUI;
    [SerializeField] private IntVariable maxInventorySlots;

    void Awake()
    {
        slotsUI = GetAllSlotsUI();
        maxInventorySlots.SetValue(slotsUI.Count);
    }

    List<InventorySlotUi> GetAllSlotsUI()
    {
        List<InventorySlotUi> allSlots = new List<InventorySlotUi>();
        foreach (Transform child in transform)
        {
            InventorySlotUi slot = child.GetComponent<InventorySlotUi>();
            if (slot != null)
            {
                allSlots.Add(slot);
            }
        }
        return allSlots;
    }

    public void Redraw()
    {
        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (i < inventory.items.Count)
                slotsUI[i].Set(inventory.items[i]);
            else
                slotsUI[i].Clear();
        }
    }
}
