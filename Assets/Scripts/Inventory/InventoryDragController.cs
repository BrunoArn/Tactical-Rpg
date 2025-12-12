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

    private InventorySlotUi dragSource;
    private InventoryContainer dragSourceContainer;
    private int dragSourceIndex = -1;

    void OnEnable()
    {
        backpackUi.EnsureSlotsCached();
        quickbarUi.EnsureSlotsCached();
        SubscribeSlots(backpackUi, true);
        SubscribeSlots(quickbarUi, false);
        HideGhost();
    }

    void OnDisable()
    {
        UnsubscribeSlots(backpackUi);
        UnsubscribeSlots(quickbarUi);
    }

    private void SubscribeSlots(InventoryUi ui, bool isBackpack)
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
        SetSource(slot);
        ShowGhost(slot);
    }

    private void OnDragging(InventorySlotUi slot, PointerEventData data)
    {
        if (dragGhost != null) dragGhost.rectTransform.position = data.position;
    }

    private void OnDroppedOn(InventorySlotUi target, PointerEventData data)
    {
        if (dragSource == null || target == null) { ClearDrag(); return; }

        var (targetContainer, targetIndex) = Resolve(target);
        Debug.Log($"Drop: src={dragSourceContainer?.name} idx={dragSourceIndex} -> dst={targetContainer?.name} idx={targetIndex}");

        if (targetContainer == null) { ClearDrag(); return; }

        //inventoryManager.TryMove(dragSourceContainer, dragSourceIndex, targetContainer, targetIndex, out var _);
        //RedrawAll();
        //ClearDrag();

        var ok = inventoryManager.TryMove(dragSourceContainer, dragSourceIndex, targetContainer, targetIndex, out var result);
        Debug.Log($"TryMove ok={ok} result={result}");
        if (ok) RedrawAll();
        ClearDrag();
    }

    private void OnDragEnded(InventorySlotUi slot, PointerEventData data)
    {
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
}
