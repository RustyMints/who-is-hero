using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DroppedItemRecord
{
    public string itemId;
    public float worldX;
    public float worldY;

    public DroppedItemRecord(string id, float x, float y)
    {
        itemId = id;
        worldX = x;
        worldY = y;
    }
}

[System.Serializable]
public class GameData 
{
    public int currency;

    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentId;

    public bool isNewGame;
    public SerializableDictionary<string, bool> checkpoints;
    public string closesCheckpointId;

    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;

    public List<DroppedItemRecord> droppedItems;

    public GameData()
    {
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;

        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        closesCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();
        droppedItems = new List<DroppedItemRecord>();
    }
}
