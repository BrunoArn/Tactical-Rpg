
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUi : MonoBehaviour,
    IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject highLight;

    [SerializeField] private Button button;

    public event Action<InventorySlotUi> Hovered;
    public event Action<InventorySlotUi> Clicked;

    public event Action<InventorySlotUi, PointerEventData> DragStarted;
    public event Action<InventorySlotUi, PointerEventData> Dragging;
    public event Action<InventorySlotUi, PointerEventData> DragEnded;
    public event Action<InventorySlotUi, PointerEventData> DroppedOn;

    public bool IsDragging { get; private set; }

    public InventorySlot SlotData { get; private set; }

    private void Awake()
    {
        if (button != null) button.onClick.AddListener(() =>
        {
            if (IsDragging) return; // ignore click when dragging
            Clicked?.Invoke(this);
        });
    }


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

    #region Drag

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (SlotData == null) return;
        IsDragging = true;
        DragStarted?.Invoke(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragging) return;
        Dragging?.Invoke(this, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDragging) return;
        IsDragging = false;
        DragEnded?.Invoke(this, eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        DroppedOn?.Invoke(this, eventData);
    }
    #endregion
}
