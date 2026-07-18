using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UIitemSlot
{
    public EquipmentType SlotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment slot -" + SlotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;

        ItemData_Equipment equipmentData = item.data as ItemData_Equipment;

        Inventory.instance.UnequipItem(equipmentData);
        Inventory.instance.AddItem(equipmentData);

        ui.itemToolTip.HideToolTip();
    }
}
