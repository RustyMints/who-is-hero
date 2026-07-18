using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float jumpAcceleration = 8f; // 跳跃时水平加速度
    
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.ConsumeCoyoteTime();
        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 跳跃时允许立即转向，保持流畅控制
        if (xInput != 0)
        {
            float targetXVelocity = player.moveSpeed * xInput;
            float currentXVelocity = rb.velocity.x;
            
            // 使用平滑加速，让转向更流畅
            float newXVelocity = Mathf.Lerp(currentXVelocity, targetXVelocity, jumpAcceleration * Time.deltaTime);
            player.SetVelocity(newXVelocity, rb.velocity.y);
        }

        if (rb.velocity.y < 0)
            stateMachine.changeState(player.airState);
    }
}
