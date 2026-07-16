using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private int defaultFontSize = 32;
    public void ShowToolTip(ItemData_Equipment item)
    {
        if (item == null)
            return;

        // ===== 汉化修改开始：物品名直接使用 asset 里 itemName 字段（已汉化改好中文名）=====
        itemNameText.text = item.itemName;
        // ===== 汉化修改：装备类型不再直接用枚举 ToString() 输出英文，改走中文映射表 =====
        itemTypeText.text = GetEquipmentTypeCN(item.equipmentType);
        // ===== 汉化修改结束：描述依然走 GetDescription（之前已完成全部中文属性名）=====
        itemDescription.text = item.GetDescription();

        AdjiusFontSize(itemNameText);
        AdjustPosition();

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        itemNameText.fontSize = defaultFontSize;

        gameObject.SetActive(false);
    }

    // ===== 汉化新增开始：EquipmentType 枚举 → 中文显示名映射方法 =====
    // 说明：
    // 1. 枚举成员名（Weapon/Armor/Amulet/Flask）仍是英文用于代码判断，避免修改导致 int 顺序错乱、
    //    asset 中已序列化的 equipmentType: 0/1/2/3 失效；只在 UI 层翻译成中文。
    // 2. 顺序与 ItemData_Equipment.cs 中的 EquipmentType 枚举严格一致：
    //    Weapon=0 武器、Armor=1 护甲、Amulet=2 护符、Flask=3 药瓶。
    // 3. 未定义的枚举值回退为「未知」，避免出现「4」「5」这种对玩家不友好的数字显示。
    private string GetEquipmentTypeCN(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon: return "武器";
            case EquipmentType.Armor:  return "护甲";
            case EquipmentType.Amulet: return "护符";
            case EquipmentType.Flask:  return "药瓶";
            default:                   return "未知";
        }
    }
    // ===== 汉化新增结束：EquipmentType 枚举 → 中文显示名映射方法 =====
}
