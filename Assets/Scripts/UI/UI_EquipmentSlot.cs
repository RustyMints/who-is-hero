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
        // ===== 修复：卸下装备UI点击空引用 + UI二次清空 =====
        // 1. Inventory 实例判空
        Inventory inventory = Inventory.GetInstance();
        if (inventory == null)
        {
            Debug.LogWarning("UI_EquipmentSlot: Inventory 实例不存在，无法卸下装备。");
            return;
        }

        // 2. item / item.data 判空（空装备槽点击直接返回）
        if (item == null || item.data == null)
            return;

        // 3. 类型转换安全检查，用 pattern matching 避免 as 转换失败传 null
        if (item.data is not ItemData_Equipment equipData)
            return;

        // 先卸下（内部已调用 UpdateSlotUI，会重绘所有装备槽，包含 CleanUpSlot）
        inventory.UnequipItem(equipData);

        // 再放回背包（AddItem 内部也会 UpdateSlotUI，刷新背包槽）
        inventory.AddItem(equipData);

        // 注意：UnequipItem 已经调用 UpdateSlotUI()，其中装备槽部分先 CleanUpSlot 再填入
        // 所以这里不需要再手动 CleanUpSlot()，否则会把刚填好的本槽UI再次清空
    }
}
