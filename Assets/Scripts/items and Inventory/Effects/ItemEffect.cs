using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : ScriptableObject
{
   public virtual void ExcuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect executed");
    }
}
