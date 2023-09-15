using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class AbilityBase : MonoBehaviour, IPoolable<AbilityBase>
{
    protected enum AbilityTimer
    {
        lifeTime
    }
    // Start is called before the first frame update
    protected ElementalSO selectedAbility;

    protected Timer timer;

    protected int CurrentPierce;

    [SerializeField] [ReadOnly]
    private List<Enemy> hitEnemy = new List<Enemy>();

    public Pool<AbilityBase> Pool { get; set; }
    public bool IsPooled { get; set; }

    protected Vector3 initialPos;
    protected Vector3 direction;
    private void ResetAbility()
    {
        initialPos = Vector3.zero;
        direction = Vector3.zero;
        selectedAbility = null;
        timer.ResetToZero();
        hitEnemy.Clear();
    }

    private void init()
    {
        if (timer == null)
        {
            timer = GameManager.Instance.TimerManager.GenerateTimers(typeof(AbilityTimer), gameObject);
        }
        timer.SetTime((int)AbilityTimer.lifeTime, selectedAbility.lifeTime);
        timer.times[(int)AbilityTimer.lifeTime].OnTimeIsZero += InvokePoolSelf;
    }

    protected virtual void InvokePoolSelf(object sender, EventArgs e)
    {
        timer.times[0].OnTimeIsZero -= InvokePoolSelf;
    }

    private void SetSelectedAbility(ElementalSO selected)
    {
        selectedAbility = selected;
        CurrentPierce = selectedAbility.pierceAmount;
        init();
        CastAbility();
    }

    public void SetSelectedAbility(ElementalSO selected, Vector3 initialPos)
    {
        this.initialPos = initialPos;
        SetSelectedAbility(selected);
    }

    public void SetSelectedAbility(ElementalSO selected, Vector3 initialPos, Vector3 dir)
    {
        direction = dir;
        SetSelectedAbility(selected, initialPos);
    }

    protected virtual void CastAbility()
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable foundEnemy;
        if (UtilityFunction.FindComponent(collision.transform,out foundEnemy))
        {
            if(!(foundEnemy as Enemy))
            {
                return;
            }
            if (hitEnemy.Contains(foundEnemy as Enemy))
            {
                return;
            }
            hitEnemy.Add(foundEnemy as Enemy);
            foundEnemy.TakeDamage(selectedAbility.damage, selectedAbility.Stagger, selectedAbility.elementType, selectedAbility.castCost);
            if (CurrentPierce > 0)
            {

                CurrentPierce--;
            }
        }
    }

    public void PoolSelf()
    {
        ResetAbility();
        Pool.PoolObj(this);
    }
}
