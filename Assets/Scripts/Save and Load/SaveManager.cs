using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;

    private GameData gameData;
    private List<IsaveManager> saveManagers;
    private FileDataHandler dataHandler;

    [ContextMenu("Delete save file")]
    private void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName,encryptData);
        dataHandler.Delete();
    }
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName,encryptData);
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

        if(this.gameData == null)
        {
            Debug.Log("未找到存档");
            NewGame();
        }
        else
        {
            gameData.isNewGame = false;
        }

        foreach(IsaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach(IsaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

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
}
