using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] public InventoryContainer quickBar;
    [SerializeField] public InventoryContainer backpack;

    public bool AddItem(ItemData item, int quantity)
    {
        if (quickBar.TryAdd(item, quantity))
            return true;
        return backpack.TryAdd(item, quantity);
    }

    public bool RemoveItem(ItemData item, int quantity = 1)
    {
        var slot = quickBar.slots.Find(s => s.item == item);
        if (slot != null)
        {
            quickBar.RemoveItem(item, quantity);
            return true;
        }

        slot = backpack.slots.Find(s => s.item == item);
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
}
