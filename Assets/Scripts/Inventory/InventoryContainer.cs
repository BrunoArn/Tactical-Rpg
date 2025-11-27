using System.Collections.Generic;
using UnityEngine;

public class InventoryContainer : MonoBehaviour
{
    public List<InventorySlot> slots = new();
    [SerializeField] int capacity = 10;

    public bool IsFull => slots.Count >= capacity;

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

    public void RemoveItem(ItemData item, int quantity = 1)
    {
        var slot = slots.Find(s => s.item == item);
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0) slots.Remove(slot);
        }
    }

    public int GetItemQuantity(ItemData item)
    {
        var slot = slots.Find(s => s.item == item);
        return slot?.quantity ?? 0;
    }

    public bool CanRemove(ItemData item, int quantity)
    {
        return GetItemQuantity(item) >= quantity;
    }

    public bool TryConsume(ItemData item, int quantity)
    {
        if(!CanRemove(item, quantity)) return false;
        RemoveItem(item, quantity);
        return true;
    }
}
