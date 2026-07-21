using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSlimeIdleState : DemonSlimeGroundState
{
    public DemonSlimeIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DemonSlime _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.PlaySFX(enemy.moveSfxIndex, enemy.transform);
        enemy.moveSfxIndex++;
        if (enemy.moveSfxIndex > 29)
            enemy.moveSfxIndex = 27;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}