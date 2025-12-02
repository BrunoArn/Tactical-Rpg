using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory container for managing item slots and quantities.
/// </summary>
public class InventoryContainer : MonoBehaviour
{
    /// <summary>
    /// List of item slots in the inventory.
    /// </summary>
    public List<InventorySlot> slots = new();

    /// <summary>
    /// Max number of distinct item slots.
    /// </summary>
    [SerializeField] int capacity = 10;

    /// <summary>
    /// True if inventory is at max capacity.
    /// </summary>
    public bool IsFull => slots.Count >= capacity;

    /// <summary>
    /// Adds quantity of item. Stacks if possible, or adds new slot if space.
    /// Returns true if added.
    /// </summary>
    /// <returns>True when the items were added (or merged into an existing slot); false otherwise.</returns>
    public bool TryAdd(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0) return false;

        if (!item.isStackable)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (IsFull) return false;
                slots.Add(new InventorySlot(item, 1));
            }
            return true;
        }

        var slot = slots.Find(s => s.item == item);
        if (slot != null)
        {
            //soma o item na slot existente
            slot.quantity += quantity;
            return true;
        }
        if (IsFull) return false;
        //adiciona novo item no slots
        slots.Add(new InventorySlot(item, quantity));
        return true;
    }

    /// <summary>
    /// Removes <paramref name="quantity"/> units of the given <paramref name="item"/>.
    /// If the remaining quantity reaches zero or less the slot is removed entirely.
    /// </summary>
    public void RemoveItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return;

        if (item.isStackable)
        {
            var slot = slots.Find(s => s.item == item);
            if (slot == null) return;

            slot.quantity -= quantity;
            if (slot.quantity <= 0) slots.Remove(slot);
            return;
        }

        for (int i = slots.Count - 1; i >= 0 && quantity > 0; i--)
        {
            var slot = slots[i];
            if (slot.item != item) continue;

            int remove = Mathf.Min(slot.quantity, quantity);
            slot.quantity -= remove;
            quantity -= remove;

            if (slot.quantity <= 0) slots.RemoveAt(i);
        }
    }

    /// <summary>
    /// Gets current quantity of item in inventory.
    /// </summary>
    public int GetItemQuantity(ItemData item)
    {
        if (item == null) return 0;

        if (item.isStackable)
        {
            return slots.Find(s => s.item == item)?.quantity ?? 0;
        }

        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.item == item) total += slot.quantity;
        }
        return total;
    }

    /// <summary>
    /// True if at least quantity of item can be removed.
    /// </summary>
    public bool CanRemove(ItemData item, int quantity)
    {
        return GetItemQuantity(item) >= quantity;
    }

    /// <summary>
    /// Tries to consume quantity of item. Returns true if successful.
    /// </summary>
    /// <returns>True when the items were successfully consumed, false otherwise.</returns>
    public bool TryConsume(ItemData item, int quantity)
    {
        if (!CanRemove(item, quantity)) return false;
        RemoveItem(item, quantity);
        return true;
    }
}
