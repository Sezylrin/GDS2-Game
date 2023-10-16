using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AcidPool : ArchProjectile
{
    // Start is called before the first frame update
    
    [SerializeField]
    private GameObject acidPool;
    
    public override void InitArch(Vector2 archDir, Vector2 targetPoint, bool fixedArrival)
    {
        base.InitArch(archDir, targetPoint, fixedArrival);

        Invoke("SpawnAcid", archDuration * 2);
    }

    public void SpawnAcid()
    {
        Speed = 0;
        initialV = 0;
        decay = 0;
        SpriteObj.gameObject.SetActive(false);
        acidPool.SetActive(true);
        col2d.enabled = true;
        //PoolSelf();
    }

    public override void PoolSelf()
    {
        SpriteObj.gameObject.SetActive(true);
        col2d.enabled = false;
        acidPool.SetActive(false);
        base.PoolSelf();
    }

    protected override void DoDamage(IDamageable target)
    {
        target.TakeDamage(damage, 0, ElementType.noElement);
    }

}
