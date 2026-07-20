using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Thunder strike", menuName = "Data/Item effect/Thunder strike")]
public class ThunderStrike_Effect : ItemEffect
{
    // ===== 修复：字段名与序列化资产(Thunder strike.asset:thunderStrikePrefab)一致，避免 fileID=0 导致的 Prefab 未赋值异常
    // 注意：修改字段名后，请在 Inspector 中重新把 ThunderStrike Prefab 拖入该字段并保存 Asset
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExcuteEffect(Transform _enemyPosition)
    {
        // ===== 修复：Prefab 未赋值时不崩溃，给出明确警告 =====
        if (thunderStrikePrefab == null)
        {
           //Debug.LogWarning($"ThunderStrike_Effect: thunderStrikePrefab 未赋值！请在 \"Data/Item Effects/Thunder strike\" 资产的 Inspector 中指定 ThunderStrike Prefab。", this);
            return;
        }
        if (_enemyPosition == null)
            return;

        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPosition.position, Quaternion.identity);
        AudioManager.instance.PlaySFX(6, _enemyPosition);
        if (newThunderStrike != null)
        {
            // 延长销毁时间到 1.5 秒，让完整动画有时间播完
            Destroy(newThunderStrike, 1.5f);
        }
    }
}
