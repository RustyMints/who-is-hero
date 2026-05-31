using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    private float controlDelay = 0.2f; // 控制延迟时间
    
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 1f;
        player.SetVelocity(5 * -player.facingDir, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 递减状态计时器
        stateTimer -= Time.deltaTime;

        // 延迟响应方向键输入，确保初始跳跃方向生效
        if (xInput != 0 && stateTimer < (1f - controlDelay))
            player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);

        if (stateTimer < 0)
            stateMachine.changeState(player.airState);

        if (player.IsGroundDetected())
            stateMachine.changeState(player.idleState);
    }
}
