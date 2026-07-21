using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IsaveManager
{
    public static GameManager instance;

    private Transform player;

    [SerializeField] private CheckPoint[] checkPoints;
    [SerializeField] private string closestCheckpointId;

    [Header("Lost currency")]
    [SerializeField] private GameObject LostCurrencyPrefab;
    public int lostCurremcyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        checkPoints = FindObjectsOfType<CheckPoint>();
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
        checkPoints = FindObjectsOfType<CheckPoint>();
        RefreshPlayerReference();
        if (SaveManager.instance != null && SaveManager.instance.gameData != null)
        {
            LoadData(SaveManager.instance.gameData);
        }
    }

    private void RefreshPlayerReference()
    {
        if (PlayerManager.instance != null && PlayerManager.instance.player != null)
            player = PlayerManager.instance.player.transform;
    }

    private void Start()
    {
        checkPoints = FindObjectsOfType<CheckPoint>();

        RefreshPlayerReference();

        if (SaveManager.instance != null && SaveManager.instance.gameData != null)
        {
            LoadData(SaveManager.instance.gameData);
        }

    }

    public void RestarScene()
    {
        SaveManager.instance.SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    private void LoadCheckPoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (CheckPoint checkPoint in checkPoints)
            {
                if (checkPoint.id == pair.Key && pair.Value)
                {
                    checkPoint.ActivateCheckPoint();
                }
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurremcyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if(lostCurremcyAmount > 0 && LostCurrencyPrefab != null)
        {
            GameObject newLostCurrency = Instantiate(LostCurrencyPrefab, new Vector3(lostCurrencyX,lostCurrencyY),Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurremcyAmount;
        }

        lostCurremcyAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(0.1f);

        LoadCheckPoints(_data);
        LoadClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurremcyAmount;

        if (player != null)
        {
            _data.lostCurrencyX = player.position.x;
            _data.lostCurrencyY = player.position.y;
        }

        CheckPoint closest = FindClosesCheckpoint();
        _data.closesCheckpointId = closest != null ? closest.id : string.Empty;

        checkPoints = FindObjectsOfType<CheckPoint>();

        if (_data.checkpoints == null)
            _data.checkpoints = new SerializableDictionary<string, bool>();

        _data.checkpoints.Clear();

        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (!string.IsNullOrEmpty(checkPoint.id))
            {
                _data.checkpoints.Add(checkPoint.id, checkPoint.activationStatus);
            }
        }

    }
    private void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.closesCheckpointId == null)
            return;

        closestCheckpointId = _data.closesCheckpointId;

        if (player == null)
            RefreshPlayerReference();

        if (player == null)
            return;

        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (closestCheckpointId == checkPoint.id)
               player.position = checkPoint.transform.position;
            
        }
    }

    private CheckPoint FindClosesCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        CheckPoint closestCheckpoint = null;

        if (player == null)
            return null;

        foreach (var checkPoint in checkPoints)
        {
            if (checkPoint == null) continue;
            float distanceToCheckpoint = Vector2.Distance(player.position, checkPoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkPoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkPoint;
            }
        }

        return closestCheckpoint;
    }

    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
