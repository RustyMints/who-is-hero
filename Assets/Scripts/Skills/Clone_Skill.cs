using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]

    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space] 
    [SerializeField] private bool canAttack;

    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool canCreateCloneOnCounterAttack;
    [Header("Clone can duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal instead of clone")]
     public bool crystalInseadOfClone;

    // [修复] 添加可选的_closeEnemy参数，避免25距离限制问题
    public void CreateClone(Transform _clonePosition,Vector3 _offset, Transform _closeEnemy = null)
    {

        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        // [修复] 当提供_closeEnemy时直接使用，否则才调用FindClosestEnemy
        Transform enemy = _closeEnemy != null ? _closeEnemy : FindClosestEnemy(newClone.transform);
        newClone.GetComponent<Clone_Skill_controller>().
            SetupClone(_clonePosition,cloneDuration,canAttack,_offset,enemy,canDuplicateClone,chanceToDuplicate,player);
    }
    // [修复结束]

    public void CreateCloneOnDashStart()
    {
        if (createCloneOnDashStart)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnDashOver()
    {
        if(createCloneOnDashOver)
            CreateClone(player.transform, Vector3.zero);
    }

    // [修复] 传递敌人transform给CreateClone，确保克隆朝向正确的敌人
    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneOnCounterAttack)
            StartCoroutine(CreateCloneWithDelay(player.transform, new Vector3(2 * player.facingDir, 0), _enemyTransform));
    }

    // [修复] 添加_enemyTransform参数，传递给CreateClone
    private IEnumerator CreateCloneWithDelay(Transform _clonePosition, Vector3 _offset, Transform _enemyTransform)
    {
        yield return new WaitForSeconds(0.4f);
            CreateClone(_clonePosition, _offset, _enemyTransform);
    }
    // [修复结束]
}
