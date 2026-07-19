using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;
    

    public virtual void GenerateDrop()
    {
        // ===== 修复：物品掉落越界问题 =====
        dropList.Clear();

        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (possibleDrop[i] != null && Random.Range(0, 100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]);
        }

        // 实际掉落数量 = 配置数量 与 候选物品数量 的较小值，避免越界
        int actualDropCount = Mathf.Min(possibleItemDrop, dropList.Count);

        for (int i = 0; i < actualDropCount; i++)
        {
            // Random.Range(int, int) 的 max 是 exclusive（不含），直接用 Count 即可
            ItemData randomItem = dropList[Random.Range(0, dropList.Count)];

            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
        // ===== 修复结束 =====
    }

    protected void DropItem(ItemData _itemData)
    {
        DropItemAt(_itemData, transform.position);
    }

    protected void DropItemAt(ItemData _itemData, Vector2 _position)
    {
        // ===== 修复：掉落物品空引用防护 =====
        if (dropPrefab == null || _itemData == null)
            return;

        GameObject newDrop = Instantiate(dropPrefab, _position, Quaternion.identity);
        if (newDrop == null)
            return;

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        ItemObject itemObj = newDrop.GetComponent<ItemObject>();
        if (itemObj != null)
            itemObj.SetupItem(_itemData, randomVelocity);
    }

    public void RespawnDropAtPosition(ItemData _itemData, Vector2 _position)
    {
        DropItemAt(_itemData, _position);
    }
}

