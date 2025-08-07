using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> items = new List<InventorySlot>();
    [SerializeField] IntVariable maxInventorySlots;

    public bool IsFull => items.Count >= maxInventorySlots.Value;

    public bool AddItem(ItemData item, int quantity)
    {
        if (IsFull && !items.Any(i => i.item == item))
            return false;

        // procura o item na lista
        var slot = items.Find(i => i.item == item);
        //achou só aumenta ele
        if (slot != null)
        {
            slot.quantity += quantity;
        }
        //nao achou adiciona ele
        else
        {
            items.Add(new InventorySlot(item, quantity));
        }   

        return true;
    }

    public void RemoveItem(ItemData item, int quantity = 1)
    {
        var slot = items.Find(i => i.item == item);
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0) items.Remove(slot);
        }
    }

}
