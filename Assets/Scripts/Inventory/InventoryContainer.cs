using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Inventory container for managing item slots and quantities.
/// </summary>
public class InventoryContainer : MonoBehaviour
{
    public List<InventorySlot> slots = new();
    public int capacity = 10;

    //public bool IsFull => slots.Count >= capacity;

    private void Start()
    {
        EnsureSize();
    }

    /// <summary>
    /// Adds quantity of item. Stacks if possible, or adds new slot if space.
    /// Returns true if added.
    /// </summary>
    /// <returns>True when the items were added (or merged into an existing slot); false otherwise.</returns>
    public bool TryAdd(ItemData item, int quantity)
    {
        EnsureSize();
        if (item == null || quantity <= 0) return false;

        if (!item.isStackable)
        {
            for (int i = 0; i < quantity; i++)
            {
                int index = slots.FindIndex(s => s == null);
                if (index < 0) return false;
                slots[index] = new InventorySlot(item, 1);
            }
            return true;
        }

        var slot = slots.Find(s => s != null && s.item == item);
        if (slot != null)
        {
            //soma o item na slot existente
            slot.quantity += quantity;
            return true;
        }
        int empty = slots.FindIndex(s => s == null);
        if (empty < 0) return false;
        //adiciona novo item no slots
        slots[empty] = new InventorySlot(item, quantity);
        return true;
    }

    /// <summary>
    /// Removes <paramref name="quantity"/> units of the given <paramref name="item"/>.
    /// If the remaining quantity reaches zero or less the slot is removed entirely.
    /// </summary>
    public void RemoveItem(ItemData item, int quantity = 1)
    {
        EnsureSize();
        if (item == null || quantity <= 0) return;

        if (item.isStackable)
        {
            var index = slots.FindIndex(s => s != null && s.item == item);
            if (index < 0) return;
            slots[index].quantity -= quantity;
            if (slots[index].quantity <= 0) slots[index] = null;
            return;
        }

        for (int i = slots.Count - 1; i >= 0 && quantity > 0; i--)
        {
            var slot = slots[i];
            if (slot == null || slot.item != item) continue;

            int remove = Mathf.Min(slot.quantity, quantity);
            slot.quantity -= remove;
            quantity -= remove;

            if (slot.quantity <= 0) slots[i] = null;
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
            return slots.Find(s => s != null && s.item == item)?.quantity ?? 0;
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

    private void EnsureSize()
    {
        slots ??= new List<InventorySlot>(capacity);
        while (slots.Count < capacity) slots.Add(null);
    }
}
