
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUi : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject highLight;

    public InventorySlot slotData { get; private set; }


    public void Set(InventorySlot slot)
    {
        icon.sprite = slot.item.icon;
        icon.enabled = true;
        slotData = slot;

        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.enabled = false;
        slotData = null;
        quantityText.text = "";
    }

    public void SetHighlight(bool value)
    {
        highLight.SetActive(value);
    }
}
