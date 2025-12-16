using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public enum MoveResult { Failed, Moved, Swapped, Merged }

    public InventoryContainer quickBar;
    public InventoryContainer backpack;

    public bool AddItem(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0) return false;
        bool hasInQuick = quickBar?.slots.Exists(s => s?.item == item) == true;
        bool hasInBack = backpack?.slots.Exists(s => s?.item == item) == true;

        //prefer stacking into the container that has the item
        if (hasInQuick && quickBar.TryAdd(item, quantity)) return true;
        if (hasInBack && backpack.TryAdd(item, quantity)) return true;

        //try adding to quickbar first if fails
        if (quickBar.TryAdd(item, quantity)) return true;
        return backpack.TryAdd(item, quantity);
    }

    public bool RemoveItem(ItemData item, int quantity = 1)
    {
        var slot = quickBar.slots.Find(s => s != null && s.item == item);
        if (slot != null)
        {
            quickBar.RemoveItem(item, quantity);
            return true;
        }

        slot = backpack.slots.Find(s => s != null && s.item == item);
        if (slot != null)
        {
            backpack.RemoveItem(item, quantity);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the total quantity of the provided <paramref name="item"/> across
    /// the quick bar and backpack combined.
    /// </summary>
    public int GetItemQuantity(ItemData item)
    {
        int q = 0;
        if (quickBar != null) q += quickBar.GetItemQuantity(item);
        if (backpack != null) q += backpack.GetItemQuantity(item);
        return q;
    }

    /// <summary>
    /// Returns true when the combined inventories contain at least the requested quantity.
    /// </summary>
    public bool CanRemove(ItemData item, int quantity)
    {
        return GetItemQuantity(item) >= quantity;
    }

    /// <summary>
    /// Try to consume <paramref name="quantity"/> of <paramref name="item"/> from the
    /// combined inventories. Consumption prefers the quickBar first, then the backpack.
    /// Returns true if all requested units were removed, false otherwise (no partial removal).
    /// </summary>
    public bool TryConsume(ItemData item, int quantity)
    {
        if (!CanRemove(item, quantity)) return false;

        // try to take from quickBar first
        int inQuick = quickBar?.GetItemQuantity(item) ?? 0;
        int consumeFromQuick = Mathf.Min(inQuick, quantity);
        if (consumeFromQuick > 0)
            quickBar.RemoveItem(item, consumeFromQuick);

        int remaining = quantity - consumeFromQuick;
        if (remaining > 0)
        {
            backpack.RemoveItem(item, remaining);
        }

        return true;
    }

    public bool TryMove(InventoryContainer from, int fromIndex, InventoryContainer to, int toIndex, int quantity, out MoveResult result)
    {
        result = MoveResult.Failed;
        if (!IsValidIndex(from, fromIndex) || !IsValidIndex(to, toIndex)) return false;

        var fromSlot = from.slots[fromIndex];
        if (fromSlot == null || quantity <= 0) return false;
        var toSlot = to.slots[toIndex];

        //merge stack
        if (toSlot != null && toSlot.item == fromSlot.item && toSlot.item.isStackable)
        {
            int moveQuantity = Mathf.Min(quantity, fromSlot.quantity);
            toSlot.quantity += moveQuantity;
            fromSlot.quantity -= moveQuantity;
            if (fromSlot.quantity <= 0) from.slots[fromIndex] = null;
            result = MoveResult.Merged;
            return true;
        }

        //move to empty
        if (toSlot == null)
        {
            int moveQuantity = Mathf.Min(quantity, fromSlot.quantity);
            to.slots[toIndex] = new InventorySlot(fromSlot.item, moveQuantity);
            fromSlot.quantity -= moveQuantity;
            if (fromSlot.quantity <= 0) from.slots[fromIndex] = null;
            result = MoveResult.Moved;
            return true;
        }

        // swap
        from.slots[fromIndex] = toSlot;
        to.slots[toIndex] = fromSlot;
        result = MoveResult.Swapped;
        return true;
    }

    //move full stack
    public bool TryMove(InventoryContainer from, int fromIndex, InventoryContainer to, int toIndex, out MoveResult result)
    {
        var slot = IsValidIndex(from, fromIndex) ? from.slots[fromIndex] : null;
        int quantity = slot?.quantity ?? 0;
        return TryMove(from, fromIndex, to, toIndex, quantity, out result);
    }

    private bool IsValidIndex(InventoryContainer container, int index)
    {
        return container != null && index >= 0 && index < container.capacity && container.slots.Count > index;
    }
}
