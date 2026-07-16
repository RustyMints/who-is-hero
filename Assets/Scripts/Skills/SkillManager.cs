using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    // [修复] 添加player变量，供Clone_Skill_controller访问玩家朝向
    public Player player { get; private set; }
    // [修复结束]
    public Dash_Skill dash { get; private set; }
    public Clone_Skill clone { get; private set; }
    public Sword_Skill sword { get; private set; }
    public Blackhole_Skill blackhole { get; private set; }
    public Crystal_Skill crystal { get; private set; }

    public Parry_Skill parry { get; private set; }

    public Dodge_Skill dodge { get; private set; }
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        // [修复] 初始化player变量
        player = PlayerManager.instance.player;
        // [修复结束]
        dash = GetComponent<Dash_Skill>(); //冲刺
        clone = GetComponent<Clone_Skill>(); //克隆，生成克隆
        sword = GetComponent<Sword_Skill>(); //刀剑
        blackhole = GetComponent<Blackhole_Skill>(); //黑洞
        crystal = GetComponent<Crystal_Skill>(); //水晶
        parry = GetComponent<Parry_Skill>(); //招架，振刀
        dodge = GetComponent<Dodge_Skill>();//躲避
    }
}
