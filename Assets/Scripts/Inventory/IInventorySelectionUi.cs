using System.Collections.Generic;
using UnityEngine;

public interface IInventorySelectionUi 
{
    InventorySlotUi CurrentSlotUi { get; }
    IReadOnlyList<InventorySlotUi> Slots { get; }
    void Redraw();
}
