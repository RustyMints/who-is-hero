using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity;
    private RectTransform myTransform;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();

        entity.onFlipped += FlipUI;
    }

    private void FlipUI()
    {
        myTransform.Rotate(0, 180, 0);
    }
}
