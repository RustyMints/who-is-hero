using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSlimeDeadState : EnemyState
{
    private Enemy_DemonSlime enemy;
    private SpriteRenderer sr;
    private float fadeOutDuration = 1f;
    private float fadeTimer;
    private Vector2 deathPosition;
    private bool isFading;

    public DemonSlimeDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DemonSlime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(30, enemy.transform);

        deathPosition = enemy.transform.position;
        enemy.cd.enabled = false;
        enemy.rb.isKinematic = true;
        enemy.rb.velocity = Vector2.zero;
        enemy.anim.applyRootMotion = false;
        sr = enemy.GetComponentInChildren<SpriteRenderer>();
        fadeTimer = 0;
        isFading = false;

        UI_HealthBar healthBar = enemy.GetComponentInChildren<UI_HealthBar>();
        if (healthBar != null)
            healthBar.StartFadeOut();
    }

    public override void Update()
    {
        base.Update();

        enemy.transform.position = deathPosition;

        if (triggerCalled && !isFading)
        {
            isFading = true;
            fadeTimer = 0;
            enemy.anim.enabled = false;
        }

        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float alpha = 1 - (fadeTimer / fadeOutDuration);
            sr.color = new Color(1, 1, 1, alpha);

            if (fadeTimer >= fadeOutDuration)
                UnityEngine.Object.Destroy(enemy.gameObject);
        }
    }
}