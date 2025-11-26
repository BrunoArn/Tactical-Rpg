using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] InventoryContainer quickBar;
    [SerializeField] InventoryContainer backpack;

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
}
