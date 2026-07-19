using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour , IsaveManager
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
    [SerializeField] private Transform statSlotParent;

    private UIitemSlot[] inventoryItemSlot;
    private UIitemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;
    private float lastTimeUseArmor;

    public float flaskCooldown { get; private set; }
    private float armorCooldown;

    public static Inventory GetInstance()
    {
        // ===== 修复：确保 instance 不为空 =====
        if (instance == null)
            instance = FindObjectOfType<Inventory>();
        return instance;
    }

    [Header("Data base")]
    public List<InventoryItem> LoadedItems;
    public List<ItemData_Equipment> loadEquipment;

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
            inventoryItemSlot = new UIitemSlot[0];

        if (stashSlotParent != null)
            stashItemSlot = stashSlotParent.GetComponentsInChildren<UIitemSlot>();
        else
            stashItemSlot = new UIitemSlot[0];

        if (equipmentSlotParent != null)
            equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        else
            equipmentSlot = new UI_EquipmentSlot[0];

        // ===== 修复：statSlot 数组初始化 —— 使用复数GetComponentsInChildren返回数组，并加null兜底，避免CS0029+NRE =====
        if (statSlotParent != null)
            statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();
        else
            statSlot = new UI_StatSlot[0];

        ApplyLoadedOrStartingItems();
    }

    private void ApplyLoadedOrStartingItems()
    {
        bool hasSaveData = loadEquipment.Count > 0 || LoadedItems.Count > 0;

        if (hasSaveData)
        {
            foreach (ItemData_Equipment item in loadEquipment)
            {
                if (item != null)
                    EquipItem(item);
            }

            foreach (InventoryItem item in LoadedItems)
            {
                if (item?.data != null)
                {
                    for (int i = 0; i < item.stackSize; i++)
                    {
                        AddItem(item.data);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < StartingItems.Count; i++)
            {
                if (StartingItems[i] != null)
                    AddItem(StartingItems[i]);
            }
        }
    }

    private void ClearAndReapplyEquipment()
    {
        if (equipmentDictionary != null)
        {
            var equippedItems = new List<ItemData_Equipment>(equipmentDictionary.Keys);
            foreach (var item in equippedItems)
            {
                equipment.Remove(equipmentDictionary[item]);
                equipmentDictionary.Remove(item);
                item.RemoveModifiers();
            }
        }

        if (inventory != null) inventory.Clear();
        if (inventoryDictiatiory != null) inventoryDictiatiory.Clear();
        if (stash != null) stash.Clear();
        if (stashDictianory != null) stashDictianory.Clear();

        ApplyLoadedOrStartingItems();

        RefreshPlayerHealth();
    }

    private void RefreshPlayerHealth()
    {
        if (PlayerManager.instance == null || PlayerManager.instance.player == null)
            return;

        CharacterStarts stats = PlayerManager.instance.player.starts;
        if (stats == null)
            return;

        stats.currentHealth = stats.GetMaxHealthValue();
        stats.onHealthChanged?.Invoke();
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

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {

            statSlot[i].UpdateStatValueUI();
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item == null)
            return;

        if (_item.itemType == ItemType.Equipment && CanAddItem())
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

    public bool CanAddItem()
    {
        if(inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("背包已满");
            return false;
        }

        return true;
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

    public void LoadData(GameData _data)
    {
        LoadedItems.Clear();
        loadEquipment.Clear();

        foreach(KeyValuePair<string,int> pair in _data.inventory)
        {
            foreach(var item in GetItemDataBase())
            {
                if(item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    LoadedItems.Add(itemToLoad);
                }
            }
        }

        foreach(string LoadedItemId in _data.equipmentId)
        {
            foreach(var item in GetItemDataBase())
            {
                if (item != null && LoadedItemId == item.itemId)
                {
                    loadEquipment.Add(item as ItemData_Equipment);
                }
            }
        }

        if (equipmentDictionary != null)
            ClearAndReapplyEquipment();
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach(KeyValuePair<ItemData,InventoryItem> pair in inventoryDictiatiory)
        {
            _data.inventory.Add(pair.Key.itemId,pair.Value.stackSize);
        }

        foreach(KeyValuePair<ItemData,InventoryItem> pair in stashDictianory)
        {
            _data.inventory.Add(pair.Key.itemId,pair.Value.stackSize);
        }

        foreach(KeyValuePair<ItemData_Equipment,InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }

    public List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetName = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach(string SOName in assetName)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
}
