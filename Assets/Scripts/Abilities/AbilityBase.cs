using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBase : MonoBehaviour , IPoolable<AbilityBase>
{
    // Start is called before the first frame update
    private ElementalSO selectedAbility;
    [SerializeField]
    private Rigidbody2D rb;

    private int CurrentPierce;

    private List<Enemy> hitEnemy = new List<Enemy>();

    public Pool<AbilityBase> Pool { get; set ; }

    private void ResetAbility()
    {
        selectedAbility = null;
        hitEnemy.Clear();
        //remember to pool ability here
    }

    public void SetSelectedAbility(ElementalSO selected)
    {
        selectedAbility = selected;
        CurrentPierce = selectedAbility.pierceAmount;
        CastAbility();
    }

    protected virtual void CastAbility()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable foundEnemy;
        if (UtilityFunction.FindComponent(collision.transform,out foundEnemy))
        {
            //do damage and stuff
            if (CurrentPierce > 0)
            {

                CurrentPierce--;
            }
        }
    }

    public void poolSelf()
    {
        Pool.PoolObj(this);
    }
}
