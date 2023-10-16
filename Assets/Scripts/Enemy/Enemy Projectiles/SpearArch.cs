using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearArch : ArchProjectile
{
    public override void InitArch(Vector2 archDir, Vector2 targetPoint, bool fixedArrival)
    {
        base.InitArch(archDir, targetPoint, fixedArrival);
        Duration = archDuration + 1;
        col2d.enabled = true;
        StartLifetime();
        Invoke("StopSpear", archDuration * 2);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.transform == shooter)
            return;
        PlayerSystem foundTarget;
        if (!UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            StopSpear();
        }
    }
    public void StopSpear()
    {
        Speed = 0;
        initialV = 0;
        decay = 0;
        col2d.enabled = false;
    }
}
