using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    private float airAcceleration = 6f; // 空中水平加速度
    private float airControl = 0.8f; // 空中控制系数
    
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.UpdateCoyoteTime();

        if (player.IsWallDetected())
            stateMachine.changeState(player.wallSlide);

        if (player.IsGroundDetected())
            stateMachine.changeState(player.idleState);

        if (Input.GetKeyDown(KeyCode.W) && player.CanJumpWithCoyoteTime())
            stateMachine.changeState(player.jumpState);

        // 空中平滑移动控制
        if (xInput != 0)
        {
            float targetXVelocity = player.moveSpeed * airControl * xInput;
            float currentXVelocity = rb.velocity.x;
            
            // 平滑加速，避免突然转向
            float newXVelocity = Mathf.Lerp(currentXVelocity, targetXVelocity, airAcceleration * Time.deltaTime);
            player.SetVelocity(newXVelocity, rb.velocity.y);
        }
        else
        {
            // 没有输入时，逐渐减速（空气阻力）
            float currentXVelocity = rb.velocity.x;
            float slowedX = Mathf.Lerp(currentXVelocity, 0, 2f * Time.deltaTime);
            player.SetVelocity(slowedX, rb.velocity.y);
        }
    }
}
