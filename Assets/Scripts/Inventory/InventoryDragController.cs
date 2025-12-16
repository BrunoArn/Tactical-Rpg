using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragController : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private InventoryUi backpackUi;
    [SerializeField] private InventoryUi quickbarUi;
    [SerializeField] private Image dragGhost;
    [SerializeField] private Vector2 ghostOffset = new Vector2(10f, -10f);

    private InventorySlotUi dragSource;
    private InventoryContainer dragSourceContainer;
    private int dragSourceIndex = -1;

    void OnEnable()
    {
        backpackUi.EnsureSlotsCached();
        quickbarUi.EnsureSlotsCached();
        SubscribeSlots(backpackUi);
        SubscribeSlots(quickbarUi);
        HideGhost();
    }

    void OnDisable()
    {
        UnsubscribeSlots(backpackUi);
        UnsubscribeSlots(quickbarUi);
    }
    #region API
    public void BeginDrag(InventorySlotUi slot)
    {
        if (slot == null) return;
        SetSource(slot);
        ShowGhost(slot);
        UpdateGhostPosition(slot);
    }
    public void DropOn(InventorySlotUi target)
    {
        if (dragSource == null || target == null) { ClearDrag(); return; }
        
        TryMoveTo(target);
    }

    public void CancelDrag()
    {
        ClearDrag();
    }

    public void UpdateGhostToSlot(InventorySlotUi slot)
    {
        UpdateGhostPosition(slot);
    }
    #endregion
    #region Mouse paths

    private void SubscribeSlots(InventoryUi ui)
    {
        if (ui?.slotsUI == null) return;
        for (int i = 0; i < ui.slotsUI.Count; i++)
        {
            var slot = ui.slotsUI[i];
            slot.DragStarted += OnDragStarted;
            slot.Dragging += OnDragging;
            slot.DragEnded += OnDragEnded;
            slot.DroppedOn += OnDroppedOn;
        }
    }

    private void UnsubscribeSlots(InventoryUi ui)
    {
        if (ui?.slotsUI == null) return;
        foreach (var slot in ui.slotsUI)
        {
            slot.DragStarted -= OnDragStarted;
            slot.Dragging -= OnDragging;
            slot.DragEnded -= OnDragEnded;
            slot.DroppedOn -= OnDroppedOn;
        }
    }

    private void OnDragStarted(InventorySlotUi slot, PointerEventData data)
    {
        BeginDrag(slot);
    }

    private void OnDragging(InventorySlotUi slot, PointerEventData data)
    {
        if (dragGhost != null) dragGhost.rectTransform.position = data.position + ghostOffset;
    }

    private void OnDroppedOn(InventorySlotUi target, PointerEventData data)
    {
        DropOn(target);
    }

    private void OnDragEnded(InventorySlotUi slot, PointerEventData data)
    {
        ClearDrag();
    }
    #endregion
    #region Core Logic
    
    private void TryMoveTo(InventorySlotUi target)
    {
        var (targetContainer, targetIndex) = Resolve(target);
        if (targetContainer == null) { ClearDrag(); return; }

        inventoryManager.TryMove(dragSourceContainer, dragSourceIndex, targetContainer, targetIndex, out var _);
        RedrawAll();
        ClearDrag();
    }

    private void SetSource(InventorySlotUi slot)
    {
        dragSource = slot;
        (dragSourceContainer, dragSourceIndex) = Resolve(slot);
    }

    private (InventoryContainer, int) Resolve(InventorySlotUi slot)
    {
        int idx = backpackUi.slotsUI.IndexOf(slot);
        if (idx >= 0) return (inventoryManager.backpack, idx);
        idx = quickbarUi.slotsUI.IndexOf(slot);
        if (idx >= 0) return (inventoryManager.quickBar, idx);
        return (null, -1);
    }

    private void ShowGhost(InventorySlotUi slot)
    {
        if (dragGhost == null || slot.SlotData?.item == null) return;
        dragGhost.sprite = slot.SlotData.item.icon;
        dragGhost.enabled = true;
    }

    private void HideGhost()
    {
        if (dragGhost != null) dragGhost.enabled = false;
    }

    private void UpdateGhostPosition(InventorySlotUi slot)
{
    if (dragGhost == null || slot == null) return;
    var rt = slot.transform as RectTransform;
    if (rt == null) return;

    // Works for Screen Space - Overlay; for Screen Space - Camera, use the canvas camera
    Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, rt.position);
    dragGhost.rectTransform.position = screenPos + (Vector3)ghostOffset;
}


    private void ClearDrag()
    {
        dragSource = null;
        dragSourceContainer = null;
        dragSourceIndex = -1;
        HideGhost();
    }

    private void RedrawAll()
    {
        backpackUi?.Redraw();
        quickbarUi?.Redraw();
    }
    #endregion
}
