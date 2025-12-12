using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{
    [SerializeField] InventoryContainer inventory;
    public List<InventorySlotUi> slotsUI;

    [SerializeField] GameObject slotUiPrefab;

    void Awake()
    {
        EnsureSlotsCached();
    }

    private void OnEnable()
    {
        Redraw();
    }

    private void CreateSlotsUI()
    {
        if (inventory == null || slotUiPrefab == null) return;

        slotsUI ??= new List<InventorySlotUi>();

        for (int i = slotsUI.Count; i < inventory.capacity; i++)
        {
            var gameObject = Instantiate(slotUiPrefab, transform);
            gameObject.transform.localScale = Vector3.one;

            var slotUI = gameObject.GetComponent<InventorySlotUi>();
            if (slotUI != null) slotsUI.Add(slotUI);

        }

    }

    public void EnsureSlotsCached()
    {
        if (slotsUI == null || slotsUI.Count == 0)
            CreateSlotsUI();
    }

    public void Redraw()
    {
        for (int i = 0; i < slotsUI.Count; i++)
        {
            var slot = i < inventory.slots.Count ? inventory.slots[i] : null;
            if (slot != null)
                slotsUI[i].Set(slot);
            else
                slotsUI[i].Clear();
        }
    }
}
