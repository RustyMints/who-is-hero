using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public void RunCoroutine(IEnumerator coroutine)
    {
        if (coroutine == null || !gameObject.activeInHierarchy || !enabled)
            return;
        
        StartCoroutine(coroutine);
    }
}