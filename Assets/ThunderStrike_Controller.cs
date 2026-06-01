using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStarts targetStats;
    [SerializeField] private float speed;

    private Animator anim;
    private bool triggered;

    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (triggered)
            return;


        transform.position = Vector2.MoveTowards(transform.position,targetStats.transform.position,speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;

        if(Vector2.Distance(transform.position,targetStats.transform.position) < 0.1f)
        {
            triggered = true;
            targetStats.TakeDamage(1);
            if(anim != null)
                anim.SetTrigger("Hit");
            Destroy(gameObject, 0.4f);
        }
    }
}
