using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bleed effect", menuName = "Data/Item effect/Bleed effect")]
public class BleedEffect : ItemEffect
{
    [SerializeField] private float bleedDuration = 2f;
    [SerializeField] private int bleedDamage = 3;

    public override void ExcuteEffect(Transform _enemyPosition)
    {
        if (_enemyPosition == null)
            return;

        CharacterStarts targetStats = _enemyPosition.GetComponent<CharacterStarts>();
        if (targetStats != null)
        {
            targetStats.SetupBleedDamage(bleedDamage);
            targetStats.ApplyBleed(bleedDuration);
        }
    }
}
