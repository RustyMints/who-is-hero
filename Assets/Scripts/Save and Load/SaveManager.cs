using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    [SerializeField] private GameObject dropPrefab;

    public GameData gameData;
    private List<IsaveManager> saveManagers;
    private FileDataHandler dataHandler;

    private static List<DroppedItemRecord> pendingDrops = new List<DroppedItemRecord>();

    public static void RecordDroppedItem(string itemId, Vector2 worldPosition)
    {
        pendingDrops.Add(new DroppedItemRecord(itemId, worldPosition.x, worldPosition.y));
    }

    [ContextMenu("删除存档")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedLoadSceneData());
    }

    private IEnumerator DelayedLoadSceneData()
    {
        yield return new WaitForEndOfFrame();

        saveManagers = FindAllSaveManagers();
        if (gameData != null)
        {
            foreach (IsaveManager saveManager in saveManagers)
            {
                saveManager.LoadData(gameData);
            }
        }

        yield return new WaitForEndOfFrame();

        RespawnDroppedItems();
    }

    private void RespawnDroppedItems()
    {
        if (gameData == null || gameData.droppedItems == null || gameData.droppedItems.Count == 0)
            return;

        if (dropPrefab == null)
        {
            Debug.LogWarning("SaveManager: dropPrefab 未设置，无法重建掉落物品。");
            return;
        }

        Inventory inventory = Inventory.instance;
        if (inventory == null)
            inventory = Inventory.GetInstance();

        if (inventory == null)
        {
            Debug.LogWarning("SaveManager: Inventory 实例不存在，无法重建掉落物品。");
            return;
        }

        List<ItemData> itemDataBase = inventory.GetItemDataBase();

        foreach (DroppedItemRecord drop in gameData.droppedItems)
        {
            if (string.IsNullOrEmpty(drop.itemId))
                continue;

            ItemData itemData = null;
            foreach (var data in itemDataBase)
            {
                if (data != null && data.itemId == drop.itemId)
                {
                    itemData = data;
                    break;
                }
            }

            if (itemData == null)
            {
                Debug.LogWarning("SaveManager: 未找到掉落物品数据 itemId=" + drop.itemId);
                continue;
            }

            GameObject newDrop = Instantiate(dropPrefab, new Vector2(drop.worldX, drop.worldY), Quaternion.identity);
            if (newDrop == null)
                continue;

            ItemObject itemObj = newDrop.GetComponent<ItemObject>();
            if (itemObj != null)
                itemObj.SetupItem(itemData, Vector2.zero);
        }

        gameData.droppedItems.Clear();
    }

    private void Start()
    {
        saveManagers = FindAllSaveManagers();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("未找到存档");
            NewGame();
        }
        else
        {
            gameData.isNewGame = false;
        }

        foreach (IsaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }

        if (SkillManager.instance != null)
            SkillManager.instance.RefreshAllSkills();
    }

    public void SaveGame()
    {
        saveManagers = FindAllSaveManagers();

        foreach (IsaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        gameData.droppedItems = new List<DroppedItemRecord>(pendingDrops);
        pendingDrops.Clear();

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IsaveManager> FindAllSaveManagers()
    {
        IEnumerable<IsaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<IsaveManager>();
        return new List<IsaveManager>(saveManagers);
    }

    public bool HasSavedData()
    {
        return dataHandler.Load() != null;
    }
}
