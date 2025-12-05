using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationUI 
{
    private readonly IList<InventorySlotUi> slotsUI;
    private readonly int columns;
    public int selectedIndex;

    public InventorySlotUi CurrentHighlightedSlot => (selectedIndex >= 0 && selectedIndex < slotsUI.Count) ? slotsUI[selectedIndex] : null;

    public NavigationUI(IList<InventorySlotUi> slotsUI, int columns, int startIndex = 0)
    {
        this.slotsUI = slotsUI;
        this.columns = Mathf.Max(1, columns);
        selectedIndex = startIndex;
    }

    public void Navigate(Vector2 input)
    {
        var col = selectedIndex % columns;
        var newIndex = selectedIndex;

        if (input.x > 0.5f && col < columns - 1) newIndex++;
        else if (input.x < -0.5f && col > 0) newIndex--;
        else if (input.y > 0.5f && selectedIndex - columns >= 0) newIndex -= columns;
        else if (input.y < -0.5f && selectedIndex + columns < slotsUI.Count) newIndex += columns;

        SetIndex(newIndex);
    }

    public void SetIndex(int index)
    {
        if (slotsUI == null || slotsUI.Count == 0) return;
        var clamped = Mathf.Clamp(index, 0, slotsUI.Count - 1);
        // no change needed, keep current highlight
        if (clamped == selectedIndex)
        {
            SetHighlight(selectedIndex, true);
            return;
        }
        SetHighlight(selectedIndex, false);
        selectedIndex = clamped;
        SetHighlight(selectedIndex, true);

    }

    public void ClearHighlights()
    {
        for (int i = 0; i < slotsUI.Count; i++) SetHighlight(i, false);
    }

    private void SetHighlight(int idx, bool value)
    {
        if (idx < 0 || idx >= slotsUI.Count) return;
        slotsUI[idx].SetHighlight(value);
        // Optionally: EventSystem.current.SetSelectedGameObject(slots[idx].gameObject) when value == true
    }
}
