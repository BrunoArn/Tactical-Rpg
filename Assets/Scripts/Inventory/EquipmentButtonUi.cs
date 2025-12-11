using UnityEngine;

public class EquipmentButtonUi : MonoBehaviour
{
    [SerializeField] EquipmentUi equipmentUi;
    [SerializeField] EquipmentSlotType slotType;

    public void OnClickRemove() => equipmentUi.RemoveEquipment(slotType);
}
