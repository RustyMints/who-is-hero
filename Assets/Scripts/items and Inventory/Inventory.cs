using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public List<ItemData> StartingItems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictiatiory;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictianory;

    [Header("Inventory UI")]

    [SerializeField] private Transform inventorySlotparent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;

    private UIitemSlot[] inventoryItemSlot;
    private UIitemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;
    private float lastTimeUseArmor;

    private float flaskCooldown;
    private float armorCooldown;

    public static Inventory GetInstance()
    {
        // ===== 修复：确保 instance 不为空 =====
        if (instance == null)
            instance = FindObjectOfType<Inventory>();
        return instance;
    }

    private void Awake()
    {
        // ===== 修复：单例互斥销毁 —— 场景有多个Inventory时保留第一个，避免UI引用被空实例覆盖导致装备UI消失 =====
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictiatiory = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictianory = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        if (inventorySlotparent != null)
            inventoryItemSlot = inventorySlotparent.GetComponentsInChildren<UIitemSlot>();
        else
        {

            inventoryItemSlot = new UIitemSlot[0];
        }

        if (stashSlotParent != null)
            stashItemSlot = stashSlotParent.GetComponentsInChildren<UIitemSlot>();
        else
        {

            stashItemSlot = new UIitemSlot[0];
        }

        if (equipmentSlotParent != null)
            equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        else
        {

            equipmentSlot = new UI_EquipmentSlot[0];
        }

        AddStartingItems();
    }

    private void AddStartingItems()
    {
        for (int i = 0; i < StartingItems.Count; i++)
        {
            AddItem(StartingItems[i]);
        }
    }

    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;
        }

        if(oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);

        newEquipment.AddModifiers();

        RemoveItem(_item);

        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
        // ===== 修复：卸下装备后刷新UI，否则装备槽残留旧图标或数据不一致 =====
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        // ===== 修复：装备槽先全部清空，再根据字典填入，防止卸下装备后UI残留旧图标 =====
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            equipmentSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].SlotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
            }
        }

        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < inventoryItemSlot.Length && i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stashItemSlot.Length && i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item == null)
            return;

        if (_item.itemType == ItemType.Equipment)
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictianory.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictianory.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictiatiory.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictiatiory.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if(inventoryDictiatiory.TryGetValue(_item,out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictiatiory.Remove(_item);
            }
            else
                value.RemoveStack();
        }

        if(stashDictianory.TryGetValue(_item,out InventoryItem stashVakue))
        {
            if(stashVakue.stackSize <= 1)
            {
                stash.Remove(stashVakue);
                stashDictianory.Remove(_item);
            }
            else
                stashVakue.RemoveStack();
        }

        UpdateSlotUI();
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft,List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();
        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictianory.TryGetValue(_requiredMaterials[i].data,out InventoryItem stashValue))
            {
                if(stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("not enough materials");
                    return false;
                }

                else
                {
                    materialsToRemove.Add(stashValue);
                }

            }

            else
            {
                Debug.Log("not enough materials");
                return false;
            }

        }

        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }

        AddItem(_itemToCraft);
        Debug.Log("Here is your item" + _itemToCraft.name);

        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentflask = GetEquipment(EquipmentType.Flask);

        if(currentflask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentflask.itemCooldown;
            currentflask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }

        else
            Debug.Log("道具正在冷却");
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUseArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUseArmor = Time.time;
            return true;
        }

        Debug.Log("盔甲正在冷却");
        return false;
    }

    
}
