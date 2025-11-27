using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple inventory container that holds a list of <see cref="InventorySlot"/> entries.
///
/// Responsibilities:
/// - Maintain the list of items and their quantities
/// - Provide helper APIs for adding, removing and consuming items
/// - Enforce a capacity limit for how many distinct slots can be stored
/// </summary>
public class InventoryContainer : MonoBehaviour
{
    /// <summary>
    /// The list of active inventory slots. Each slot maps an <see cref="ItemData"/>
    /// to a quantity.
    /// </summary>
    public List<InventorySlot> slots = new();

    /// <summary>
    /// Maximum number of distinct item slots the container can hold.
    /// This limits how many different item types can be present simultaneously.
    /// </summary>
    [SerializeField] int capacity = 10;

    /// <summary>
    /// Returns true when the container has reached its maximum number of distinct
    /// item slots (capacity).
    /// </summary>
    public bool IsFull => slots.Count >= capacity;

    /// <summary>
    /// Attempt to add <paramref name="quantity"/> of <paramref name="item"/> to
    /// the container.
    ///
    /// Behavior:
    /// - If the item already exists in an existing slot, the quantity of that slot
    ///   is increased and the method returns true.
    /// - If the item does not exist and the container is not full, a new
    ///   <see cref="InventorySlot"/> is created and the method returns true.
    /// - If the container is full and the item does not already exist, the method
    ///   returns false and the item is not added.
    /// </summary>
    /// <returns>True when the items were added (or merged into an existing slot); false otherwise.</returns>
    public bool TryAdd(ItemData item, int quantity)
    {
        var slot = slots.Find(s => s.item == item);
        if (slot != null)
        {
            //soma o item na slot existente
            slot.quantity += quantity;
            return true;
        }
        if(IsFull) return false;
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
        var slot = slots.Find(s => s.item == item);
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0) slots.Remove(slot);
        }
    }

    /// <summary>
    /// Returns the current quantity of <paramref name="item"/> in the container
    /// (0 when no slot for this item exists).
    /// </summary>
    public int GetItemQuantity(ItemData item)
    {
        var slot = slots.Find(s => s.item == item);
        return slot?.quantity ?? 0;
    }

    /// <summary>
    /// Returns true if the container currently contains at least
    /// <paramref name="quantity"/> of <paramref name="item"/> and the items
    /// can be removed.
    /// </summary>
    public bool CanRemove(ItemData item, int quantity)
    {
        return GetItemQuantity(item) >= quantity;
    }

    /// <summary>
    /// Convenience method to attempt to consume <paramref name="quantity"/>
    /// of <paramref name="item"/>. Uses <see cref="CanRemove"/> to check
    /// availability and then removes the items if possible.
    /// </summary>
    /// <returns>True when the items were successfully consumed, false otherwise.</returns>
    public bool TryConsume(ItemData item, int quantity)
    {
        if(!CanRemove(item, quantity)) return false;
        RemoveItem(item, quantity);
        return true;
    }
}
