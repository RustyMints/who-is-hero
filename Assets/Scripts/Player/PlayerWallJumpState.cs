using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    private float controlDelay = 0.05f; // 减少控制延迟，让操作更流畅
    private float wallJumpAirControl = 0.7f; // 墙壁跳跃空中控制系数
    
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 0.5f; // 缩短状态时间，更快进入普通空中状态
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

        // 快速响应方向键输入，提供更好的控制感
        if (stateTimer < (0.5f - controlDelay))
        {
            if (xInput != 0)
            {
                float targetXVelocity = player.moveSpeed * wallJumpAirControl * xInput;
                float currentXVelocity = rb.velocity.x;
                
                // 平滑转向
                float newXVelocity = Mathf.Lerp(currentXVelocity, targetXVelocity, 8f * Time.deltaTime);
                player.SetVelocity(newXVelocity, rb.velocity.y);
            }
            else
            {
                // 空气阻力减速
                float currentXVelocity = rb.velocity.x;
                float slowedX = Mathf.Lerp(currentXVelocity, 0, 3f * Time.deltaTime);
                player.SetVelocity(slowedX, rb.velocity.y);
            }
        }

        if (stateTimer < 0)
            stateMachine.changeState(player.airState);

        if (player.IsGroundDetected())
            stateMachine.changeState(player.idleState);
    }
}
