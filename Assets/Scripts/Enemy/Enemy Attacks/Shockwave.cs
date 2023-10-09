using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [field: SerializeField] private Rhino Parent { get; set; }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        IDamageable foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (foundTarget is PlayerSystem)
            {
                PlayerSystem temp = foundTarget as PlayerSystem;
                if (temp.GetState() == playerState.perfectDodge)
                {
                    temp.InstantRegenPoint();
                    temp.CounterSuccesful(Parent);
                }
                else
                {
                    Parent.DoDamage(foundTarget);
                }
            }
            else
            {
                Parent.DoDamage(foundTarget);
            }
        }
    }*/
}
