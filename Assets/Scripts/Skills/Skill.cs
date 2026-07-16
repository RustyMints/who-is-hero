using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
     public float cooldown;
     protected float cooldowmTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
    }
    protected virtual void Update()
    {
        cooldowmTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if(cooldowmTimer < 0)
        {
            //使用技能
            UseSkill();
            cooldowmTimer = cooldown;
            return true;
        }

        Debug.Log("技能正在冷却");
        return false;
    }

    public virtual void UseSkill()
    {
        //做一些特定的事情
    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closesDistance = Mathf.Infinity;
        Transform closesEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);

                if (distanceToEnemy < closesDistance)
                {
                    closesDistance = distanceToEnemy;
                    closesEnemy = hit.transform;
                }
            }
        }

        return closesEnemy;
    }
}
