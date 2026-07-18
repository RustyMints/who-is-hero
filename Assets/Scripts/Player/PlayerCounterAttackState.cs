using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack", false);

        player.skill.parry.StartCooldown();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10; //设置足够长的时间让动画播放完
                    player.anim.SetBool("SuccessfulCounterAttack", true);

                    player.skill.parry.UseSkill();

                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        // ===== 镜像反击方向修复开始：这里必须使用 Enemy 根对象的 transform，
                        // ===== 而不是 Collider2D 的 hit.transform（碰撞体可能挂在子物体上，位置有偏差）。
                        // ===== 否则后续 FaceClosestTarget 用子物体的位置去和克隆位置比较，
                        // ===== 会导致朝向判断错（比如克隆在敌人"根"的右侧、却在"子碰撞体"的左侧）。
                        Enemy enemy = hit.GetComponent<Enemy>();
                        player.skill.parry.MakeMirageOnParry(enemy != null ? enemy.transform : hit.transform);
                        // ===== 镜像反击方向修复结束
                    }
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
            stateMachine.changeState(player.idleState);

    }
}
