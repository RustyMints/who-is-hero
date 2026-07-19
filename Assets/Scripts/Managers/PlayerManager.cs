using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IsaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public bool HaveEnoughMoney(int _price)
    {
        if(_price > currency)
        {
            Debug.Log("金币不足");
            return false;
        }

        currency = currency - _price;
        return true;
    }

    public int GetCurrency() => currency;

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
