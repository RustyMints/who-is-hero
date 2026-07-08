using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item effect")]
public class ItemEffect : ScriptableObject
{
   public virtual void ExcuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect executed");
    }
}
