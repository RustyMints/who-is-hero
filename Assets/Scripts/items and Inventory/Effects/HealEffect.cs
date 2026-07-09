using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect",menuName = "Data/Item effect/Heal effect")]
public class HealEffect : ItemEffect
{
    [Range(0f,1f)]
    [SerializeField] private float healPercent;
    public override void ExcuteEffect(Transform _enemyPosition)
    {
        //获取玩家信息
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //根据实际情况决定治疗量
        int healAmount = Mathf.RoundToInt( playerStats.GetMaxHealthValue() * healPercent) ;

        //给予治疗
        playerStats.IncreaseHealthBy(healAmount);
    }
}
