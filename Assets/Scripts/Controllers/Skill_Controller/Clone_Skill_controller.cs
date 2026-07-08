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

    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack,Vector3 _offset,Transform _closesEnemy,bool _canDuplicate,float _chanceToDuplicate,Player _player)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

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
                player.starts.DoDamage(hit.GetComponent<CharacterStarts>());
                

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
                facingDir = 1;
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
