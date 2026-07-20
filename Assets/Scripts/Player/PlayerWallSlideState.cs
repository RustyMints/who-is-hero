using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if(player.IsWallDetected() == false )
            stateMachine.changeState(player.airState);

        if (Input.GetKeyDown(KeyCode.W))
        {
            stateMachine.changeState(player.wallJump);
            return;
        }
        
        if (xInput != 0 && player.facingDir != xInput)
              stateMachine.changeState(player.idleState);

        if (yInput < 0)
            player.SetVelocity(0, rb.velocity.y);
        else
            player.SetVelocity(0, rb.velocity.y * 0.7f);

        if (player.IsGroundDetected())
              stateMachine.changeState(player.idleState);
        
    }
}
