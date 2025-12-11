using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{
    [SerializeField] InventoryContainer inventory;
    public List<InventorySlotUi> slotsUI;

    [SerializeField] InventoryInteractionController controller;
    [SerializeField] GameObject slotUiPrefab;

    void Awake()
    {
        //slotsUI = GetAllSlotsUI();
        CreateSlotsUI();
    }

    private void OnEnable()
    {
        Redraw();
    }

    private void CreateSlotsUI()
    {
        if (inventory == null || slotUiPrefab == null) return;

        slotsUI ??= new List<InventorySlotUi>();
        int cap = inventory.capacity;

        for (int i = slotsUI.Count; i < cap; i++)
        {
            var gameObject = Instantiate(slotUiPrefab, transform);
            gameObject.transform.localScale = Vector3.one;
            var slotUI = gameObject.GetComponent<InventorySlotUi>();
            if (slotsUI == null) return;

            if(controller != null) slotUI.Clicked += controller.OnSlotCliked;
            
            slotsUI.Add(slotUI);


        }

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
