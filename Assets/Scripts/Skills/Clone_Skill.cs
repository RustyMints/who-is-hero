using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkilltreeSlot cloneAttaclUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkilltreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("Multip clone")]
    [SerializeField] private UI_SkilltreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkilltreeSlot crystalInseadUnlockButton;
     public bool crystalInseadOfClone;

    protected override void Start()
    {
        base.Start();

        cloneAttaclUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultClone);
        crystalInseadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    #region Ulock region
    public override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultClone();
        UnlockCrystalInstead();
    }
    private void UnlockCloneAttack()
    {
        if (cloneAttaclUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void UnlockMultClone()
    {
        if (multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;

        }

    }

    private void UnlockCrystalInstead()
    {
        if (crystalInseadUnlockButton.unlocked)
        {
            crystalInseadOfClone = true;
        }

    }

    #endregion

    // [修复] 添加可选的_closeEnemy参数，避免25距离限制问题；新增_preferredTargetEnemy：镜像反击/精确命中场景可指定锁定的特定敌人
    public void CreateClone(Transform _clonePosition,Vector3 _offset, Transform _preferredTargetEnemy = null)
    {

        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        // ===== 镜像反击方向修复开始：在调用 FindClosestEnemy 前，先把新克隆临时摆到真实生成位置
        // ===== 修复前：newClone.transform.position 还是 prefab 的默认位置（通常 0,0,0）
        // ===== FindClosestEnemy 在默认位置找敌人 → 要么 null 要么找错 → 克隆方向错
        newClone.transform.position = _clonePosition.position + _offset;
        Transform closest = _preferredTargetEnemy != null ? _preferredTargetEnemy : FindClosestEnemy(newClone.transform);
        // ===== 镜像反击方向修复结束

        newClone.GetComponent<Clone_Skill_controller>().
            SetupClone(_clonePosition,cloneDuration,canAttack,_offset,closest,canDuplicateClone,chanceToDuplicate,player,attackMultiplier);
    }
    
    public void CreateCloneWithDelay(Transform _enemyTransform, Transform _preferredTargetEnemy = null)
    {
        // ===== 镜像反击方向修复开始：offset 不再依赖 player.facingDir（假设玩家敌人严格面对面的脆弱逻辑）
        // ===== 改为基于敌人与玩家在世界坐标系中的真实相对位置：
        // =====   dirX = 敌人.x - 玩家.x
        // =====     > 0 → 敌人在玩家右边 → 敌人"身后"（远离玩家一侧）是更右边 → +2
        // =====     < 0 → 敌人在玩家左边 → 敌人"身后"是更左边 → -2
        // ===== 这样无论玩家当前 facingDir 朝哪、动画是否已经翻转完毕，
        // ===== 克隆都一定落在真正的敌人身后，FaceClosestTarget 再根据 closesEnemy 与克隆位置比较朝向，
        // ===== 就能确保克隆面朝敌人（不会出现克隆在敌人与玩家之间却判断成面朝敌人的情况）。
        float dirX = _enemyTransform.position.x - player.transform.position.x;
        float sign = dirX == 0f ? player.facingDir : Mathf.Sign(dirX);
        Vector3 behindEnemyOffset = new Vector3(sign * 1.5f, 0);
        StartCoroutine(CloneDelayCorotine(_enemyTransform, behindEnemyOffset, _preferredTargetEnemy));
        // ===== 镜像反击方向修复结束
    }

    // [修复] 添加_enemyTransform参数，传递给CreateClone；透传首选目标敌人_preferredTargetEnemy
    private IEnumerator CloneDelayCorotine(Transform _transform, Vector3 _offset, Transform _preferredTargetEnemy = null)
    {
        yield return new WaitForSeconds(0.4f);
            CreateClone(_transform,_offset, _preferredTargetEnemy);
    }
    // [修复结束]
}
