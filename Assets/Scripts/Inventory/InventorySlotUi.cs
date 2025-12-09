
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUi : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject highLight;

    public event Action<InventorySlotUi> Hovered;

    public InventorySlot SlotData { get; private set; }


    public void Set(InventorySlot slot)
    {
        icon.sprite = slot.item.icon;
        icon.enabled = true;
        SlotData = slot;

        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.enabled = false;
        SlotData = null;
        quantityText.text = "";
    }

    public void SetHighlight(bool value)
    {
        highLight.SetActive(value);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hovered?.Invoke(this);
    }
}
