using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : itemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;
    public override void GenerateDrop()
    {
        // ===== 修复：Player死亡掉落装备空引用问题 =====
        Inventory inventory = Inventory.instance;
        if (inventory == null)
            inventory = Inventory.GetInstance();

        if (inventory == null)
        {
            Debug.LogWarning("PlayerItemDrop: Inventory 实例不存在，无法掉落装备。");
            return;
        }

        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materulsToLoose = new List<InventoryItem>();
        
        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            if (item == null || item.data == null)
                continue;

            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }

        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            if (itemsToUnequip[i]?.data is ItemData_Equipment equipData)
                inventory.UnequipItem(equipData);
        }
        // ===== 修复结束 =====

        foreach (InventoryItem item in inventory.GetStashList())
        {
            if(Random.Range(0, 100) <= chanceToLooseMaterials)
            {
                DropItem(item.data);
                materulsToLoose.Add(item);
            }
        }

        for (int i = 0; i < materulsToLoose.Count; i++)
        {
            inventory.RemoveItem(materulsToLoose[i].data);
        }
    }
}
