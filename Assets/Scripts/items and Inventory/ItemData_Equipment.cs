using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EquipmentType
{
    Weapon,      // 武器
    Armor,       // 护甲
    Amulet,      // 护符
    Flask        // 药瓶
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemCooldown;
    public ItemEffect[] itemEffects;
    [TextArea]
    public string itemEffectDescription;


    [Header("Major stats")]         // 主要属性 - 影响角色基础能力
    public int strength;            // 力量 - 增加物理攻击和物理防御
    public int agility;             // 敏捷 - 增加闪避和暴击几率
    public int intelligence;        // 智力 - 增加魔法伤害和魔法抗性
    public int vitality;            // 活力 - 增加最大生命值

    [Header("offensive stats")]     // 攻击属性 - 影响伤害输出
    public int damage;              // 伤害 - 增加基础物理伤害
    public int critChance;          // 暴击几率 - 攻击造成暴击的概率
    public int critPower;           // 暴击伤害 - 暴击时的伤害倍率加成

    [Header("Defensive stats")]     // 防御属性 - 影响生存能力
    public int health;              // 生命值 - 直接增加最大生命值
    public int armor;               // 护甲 - 减少受到的物理伤害
    public int evasion;             // 闪避 - 完全躲避攻击的几率
    public int magicResistance;     // 魔法抗性 - 减少受到的魔法伤害

    [Header("Magic stats")]         // 魔法属性 - 影响元素伤害
    public int fireDamage;          // 火焰伤害 - 增加火焰元素伤害
    public int iceDamage;           // 冰霜伤害 - 增加冰霜元素伤害
    public int lightingDamage;      // 雷电伤害 - 增加雷电元素伤害

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    private int DescriptionLength;

    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExcuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);
    }
    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamage.RemoveModifier(lightingDamage);
    }

    public override string GetDescription()
    {
        // ===== 修复：装备描述拼接前先清空StringBuilder，防止多次查看时描述逐行累积 =====
        //sb.Length = 0;
        sb.Clear();
        DescriptionLength = 0;

        // Major stats —— 主要属性
        AddItemDescription(strength, "力量");
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智力");
        AddItemDescription(vitality, "活力");

        // Offensive stats —— 攻击属性
        AddItemDescription(damage, "伤害");
        AddItemDescription(critChance, "暴击几率");
        AddItemDescription(critPower, "暴击伤害");

        // Defensive stats —— 防御属性
        AddItemDescription(health, "生命值");
        AddItemDescription(armor, "护甲");
        AddItemDescription(evasion, "闪避");
        AddItemDescription(magicResistance, "魔法抗性");

        // Magic stats —— 魔法属性
        AddItemDescription(fireDamage, "火焰伤害");
        AddItemDescription(iceDamage, "冰霜伤害");
        AddItemDescription(lightingDamage, "雷电伤害");


        if (DescriptionLength < 5)
        {
            for (int i =  0; i < 5 - DescriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        if(itemEffectDescription.Length > 0)
        {
            sb.AppendLine();
            sb.Append(itemEffectDescription);
        }

        return sb.ToString();
    }

    private void AddItemDescription (int _value,string _name)
    {
        if(_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            if (_value > 0)
                sb.Append("+ " + _value + " " + _name);

            DescriptionLength++;
        }
    }
}
