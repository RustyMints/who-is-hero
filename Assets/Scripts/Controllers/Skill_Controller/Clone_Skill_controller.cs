using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill_controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;

    private float cloneTimer;
    private float attackMultiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float  attackCheckRadius = 0.8f;
    private Transform closesEnemy;
    private int facingDir = 1;


    private bool canDuplicateClone;
    private float chanceToDuplicate;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if(cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }

    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack,Vector3 _offset,Transform _closesEnemy,bool _canDuplicate,float _chanceToDuplicate,Player _player,float _attackMultiplier)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        attackMultiplier = _attackMultiplier;
        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        closesEnemy = _closesEnemy;
        chanceToDuplicate = _chanceToDuplicate;
        canDuplicateClone = _canDuplicate;
        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //player.starts.DoDamage(hit.GetComponent<CharacterStarts>());
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.cloneDoDamage(enemyStats, attackMultiplier);

                if (player.skill.clone.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)
                        weaponData.Effect(hit.transform);
                }

                if (canDuplicateClone)
                {
                    if(Random.Range(0,100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(0.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    // [修复] 优化FaceClosestTarget方法，确保facingDir正确设置，无敌人时使用玩家朝向
    private void FaceClosestTarget()
    {
        // ===== 镜像反击方向修复开始：翻转前先清空 rotation，避免多次 Rotate(0,180,0) 累加翻转导致方向错乱
        // ===== （例如克隆上次朝左翻转180后，这次该朝右却不清零，就变成360°看似朝右实际还是左）
        transform.rotation = Quaternion.identity;
        // ===== 镜像反击方向修复结束

        if(closesEnemy != null)
        {
            if (transform.position.x > closesEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
            else
            {
                // [修复] 添加else分支，确保facingDir在右侧时也正确设置
                // ===== 镜像反击方向修复：右分支已在函数开头清空 rotation.y=0，此处只需 facingDir=1 即可保持朝右
                facingDir = 1;
                // ===== 镜像反击方向修复结束
            }
        }
        else
        {
            // [修复] 无敌人时使用玩家朝向
            facingDir = SkillManager.instance.player.facingDir;
            if (facingDir == -1)
            {
                transform.Rotate(0, 180, 0);
            }
        }
    }
    // [修复结束]
}
