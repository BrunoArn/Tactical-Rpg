
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUi : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void Set(InventorySlot slot)
    {
        icon.sprite = slot.item.icon;
        icon.enabled = true;

        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.enabled = false;
        quantityText.text = "";
    }
}
