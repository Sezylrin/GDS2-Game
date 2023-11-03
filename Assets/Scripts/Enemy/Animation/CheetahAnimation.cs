using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheetahAnimation : EnemyAnimation
{
    public void ChargingThree(Vector2 dir)
    {
        anim.SetFloat("Charge", (float)CustomMath.GetDirection(Vector2.right, dir) / 4f);
        anim.Play("ChargingThree");
    }

    public void AttackingThree(Vector2 dir)
    {
        anim.SetFloat("Attack", (float)CustomMath.GetDirection(Vector2.right, dir) / 4f);
        anim.Play("AttackingThree");
    }
}
