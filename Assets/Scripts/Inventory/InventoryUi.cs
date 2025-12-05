using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUi : MonoBehaviour
{
    [SerializeField] InventoryContainer inventory;
    public List<InventorySlotUi> slotsUI;

    void Awake()
    {
        slotsUI = GetAllSlotsUI();
    }

    private void OnEnable() {
        Redraw();
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

    public void EnsureSlotsCached()
{
    if (slotsUI == null || slotsUI.Count == 0)
        slotsUI = new List<InventorySlotUi>(GetComponentsInChildren<InventorySlotUi>(true));
}

    public void Redraw()
    {
        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (i < inventory.slots.Count)
                slotsUI[i].Set(inventory.slots[i]);
            else
                slotsUI[i].Clear();
        }
    }
}
