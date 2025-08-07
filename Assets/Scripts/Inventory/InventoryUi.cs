using System.Collections.Generic;
using UnityEngine;

public class InventoryUi : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    public List<InventorySlotUi> slotsUI;
    [SerializeField] private IntVariable maxInventorySlots;

    void Awake()
    {
        maxInventorySlots.SetValue(slotsUI.Count);
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
