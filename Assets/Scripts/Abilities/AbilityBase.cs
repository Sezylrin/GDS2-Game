using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class AbilityBase : MonoBehaviour, IPoolable<AbilityBase>
{
    // Start is called before the first frame update
    protected ElementalSO selectedAbility;
    [SerializeField]
    protected Rigidbody2D rb;

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
            timer = TimerManager.Instance.GenerateTimers(3, gameObject);
        }
        timer.SetTime(0, selectedAbility.lifeTime);
        timer.OnTimeIsZero += InvokePoolSelf;
    }

    protected virtual void InvokePoolSelf(object sender, Timer.OnTimeIsZeroEventArgs e)
    {
        Debug.Log("lifetime reached");
        timer.OnTimeIsZero -= InvokePoolSelf;
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
            if (hitEnemy.Contains(foundEnemy as Enemy))
            {
                return;
            }
            hitEnemy.Add(foundEnemy as Enemy);
            //do damage and stuff
            if (CurrentPierce > 0)
            {

                CurrentPierce--;
            }
        }
    }

    public void poolSelf()
    {
        ResetAbility();
        Pool.PoolObj(this);
    }
}
