using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DemonSlime : Enemy
{
    #region States

    public DemonSlimeIdleState idleState { get; private set; }
    public DemonSlimeMoveState moveState { get; private set; }
    public DemonSlimeBattleState battleState { get; private set; }
    public DemonSlimeAttackState attackState { get; private set; }
    public DemonSlimeStunnedState stunnedState { get; private set; }
    public DemonSlimeDeadState deadState { get; private set; }

    #endregion

    public int moveSfxIndex = 27;

    protected override void Awake()
    {
        base.Awake();

        facingDir = -1;
        facingRight = false;

        idleState = new DemonSlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new DemonSlimeMoveState(this, stateMachine, "Walk", this);
        battleState = new DemonSlimeBattleState(this, stateMachine, "Walk", this);
        attackState = new DemonSlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new DemonSlimeStunnedState(this, stateMachine, "Stunned", this);
        deadState = new DemonSlimeDeadState(this, stateMachine, "Dead", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Intialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    
}