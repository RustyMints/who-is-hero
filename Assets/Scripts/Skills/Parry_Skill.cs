using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_SkilltreeSlot parryUnlockButton;
    public bool parryUnlocked {  get; private set; }

    [Header("Parry restore")]
    [SerializeField] private UI_SkilltreeSlot restoreUnlockButton;
    [Range(0f,1f)]
    [SerializeField] private float restoreHealthPerentage;
    public bool restoreUnlocked { get; private set; }

    [Header("Parry with mirage")]
    [SerializeField] private UI_SkilltreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked { get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.starts.GetMaxHealthValue() * restoreHealthPerentage);
            player.starts.IncreaseHealthBy(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryRestore);
        parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);
    }

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryRestore();
        UnlockParryWithMirage();
    }
    private void UnlockParry()
    {
        if(parryUnlockButton.unlocked)
            parryUnlocked = true;
    }

    private void UnlockParryRestore()
    {
        if(restoreUnlockButton.unlocked)
            restoreUnlocked = true;
    }

    private void UnlockParryWithMirage()
    {
        if(parryWithMirageUnlockButton.unlocked)
            parryWithMirageUnlocked = true;
    }

    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if (parryWithMirageUnlocked)
            // ===== 镜像反击方向修复开始：
            // ===== 参数1：_respawnTransform = 被反击的敌人根 transform，作为克隆生成的位置基准（敌人身后 2 格）
            // ===== 参数2：_preferredTargetEnemy = 同一个敌人根 transform，作为克隆锁定的优先目标
            // =====   · 这样就不会在克隆 Instantiate 后默认位置 (0,0,0) 调 FindClosestEnemy 找到 null 或错敌
            // =====   · 同时也不会因为 25 单位范围限制在远距离镜像反击场景下丢目标
            // =====   · 配合 Clone_Skill.CreateCloneWithDelay 中基于"敌人-玩家真实相对位置"算的 behindEnemyOffset，
            // =====     克隆必落在真正的敌人身后，再结合 Clone_Skill_controller.FaceClosestTarget（先清 rotation.y=0 再判断），
            // =====     就能保证克隆正确面朝敌人攻击、不会背对。
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform, _preferredTargetEnemy: _respawnTransform);
            // ===== 镜像反击方向修复结束
    }
}
