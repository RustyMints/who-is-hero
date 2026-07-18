using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if(Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackholeUnlocked && player.skill.blackhole.IsSkillReady())
            stateMachine.changeState(player.blackHole);

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlock)
            stateMachine.changeState(player.aimSword);

        if (Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked && player.skill.parry.IsSkillReady())
            stateMachine.changeState(player.counterAttack);

        if (Input.GetKey(KeyCode.Mouse0))
            stateMachine.changeState(player.primaryAttack);

        if (!player.IsGroundDetected())
            stateMachine.changeState(player.airState);

        if (Input.GetKeyDown(KeyCode.W) && player.IsGroundDetected())
        {
            player.ResetCoyoteTime();
            stateMachine.changeState(player.jumpState);
        }
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
