using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ice and Fire Effect", menuName = "Data/Item effect/Ice and Fire")]
public class IceAndFireEffect : ItemEffect
{
    [SerializeField] private GameObject IceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override void ExcuteEffect(Transform _respondPosition)
    {
        Player player = PlayerManager.instance.player;

        //冰与火之歌会在第三次攻击时触发
        bool thirdAttack = player.primaryAttack.comboCounter == 2;

        if (thirdAttack)
        {
            GameObject newIceAmdFire = Instantiate(IceAndFirePrefab,_respondPosition.position,player.transform.rotation);
            newIceAmdFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir,0);

            Destroy(newIceAmdFire, 10);
        }

    }
}
