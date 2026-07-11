using UnityEngine.EventSystems;

public class UI_CraftSlot : UIitemSlot
{
    protected override void Start()
    {
        base.Start();

    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
            return;

        item.data = _data;

        itemImage.sprite = _data.itemicon;
        // ===== 汉化修改开始：原代码使用 _data.name（资源名，英文），
        // ===== 改为 _data.itemName（玩家显示名，.asset 字段里已改中文），保证制造列表中文显示 =====
        itemText.text = _data.itemName;
        // ===== 汉化修改结束 =====

        if (itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * 0.7f;
        else
            itemText.fontSize = 24;
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}
