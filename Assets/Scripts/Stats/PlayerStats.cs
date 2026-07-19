using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStarts
{
    private Player player;

    [Header("Death penalty")]
    [SerializeField][Range(0f, 1f)] private float minCurrencyLoss = 0.25f;
    [SerializeField][Range(0f, 1f)] private float maxCurrencyLoss = 0.50f;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>(); 
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        float lossPercent = Random.Range(minCurrencyLoss, maxCurrencyLoss);
        int lostAmount = Mathf.RoundToInt(PlayerManager.instance.currency * lossPercent);
        if (lostAmount > 0)
        {
            GameManager.instance.lostCurremcyAmount = lostAmount;
            PlayerManager.instance.currency -= lostAmount;
        }

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.Effect(player.transform);
    }

    public override void OnEvasion()
    {
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void cloneDoDamage(CharacterStarts _targetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);
        

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
        //DomagicalDamage(_targetStats);

        DomagicalDamage(_targetStats);
    }
}
