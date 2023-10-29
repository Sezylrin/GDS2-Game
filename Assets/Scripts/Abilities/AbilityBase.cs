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
    protected List<Enemy> hitEnemy = new List<Enemy>();

    public Pool<AbilityBase> Pool { get; set; }
    public bool IsPooled { get; set; }

    protected Vector3 initialPos;
    protected Vector3 direction;

    protected int finalDamage;
    protected int finalStagger;

    protected Transform followTR;

    [SerializeField]
    protected Animator anim;
    private void ResetAbility()
    {
        initialPos = Vector3.zero;
        direction = Vector3.zero;
        selectedAbility = null;
        timer.ResetToZero();
        hitEnemy.Clear();
    }

    public void init()
    {
        if(timer == null)
            timer = GameManager.Instance.TimerManager.GenerateTimers(typeof(AbilityTimer), gameObject);
        timer.times[(int)AbilityTimer.lifeTime].OnTimeIsZero += InvokePoolSelf;
    }

    private void InvokePoolSelf(object sender, EventArgs e)
    {
        PoolSelf();
    }

    private void SetSelectedAbility(ElementalSO selected)
    {
        selectedAbility = selected;
        CurrentPierce = selectedAbility.pierceAmount;
        timer.SetTime((int)AbilityTimer.lifeTime, selectedAbility.lifeTime);
        if (GameManager.Instance.PCM.system.isCountered)
        {
            finalDamage = GameManager.Instance.PCM.system.ModifyDamage(selected.damage);
            finalStagger = GameManager.Instance.PCM.system.ModifyStagger(selected.Stagger);
            GameManager.Instance.PCM.system.CounterUsed();
        }
        else
        {
            finalDamage = selected.damage;
            finalStagger = selected.Stagger;
        }    
        CastAbility();
    }

    public void SetSelectedAbility(ElementalSO selected, Vector3 initialPos)
    {
        this.initialPos = initialPos;
        SetSelectedAbility(selected);
    }

    public void SetSelectedAbility(ElementalSO selected, Vector3 initialPos, Vector3 dir, Transform follow = null)
    {
        followTR = follow;
        direction = dir;
        SetSelectedAbility(selected, initialPos);
    }


    protected virtual void CastAbility()
    {
        anim.Play(selectedAbility.elementType.ToString());
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (CurrentPierce <= 0)
            return;
        Enemy foundEnemy;
        if (UtilityFunction.FindComponent(collision.transform,out foundEnemy))
        {           
            
            if (hitEnemy.Contains(foundEnemy))
            {
                return;
            }
            hitEnemy.Add(foundEnemy);
            GameManager.Instance.AudioManager.PlaySound(selectedAbility.audioHit);
            foundEnemy.TakeDamage(finalDamage, finalStagger, selectedAbility.elementType, selectedAbility.castCost);
            //GameManager.Instance.PCM.system.AddToConsumeBar(selectedAbility.consumePoints);
            Vector3 dir;
            if (direction.Equals(Vector3.zero))
            { 
                dir = collision.transform.position - initialPos;
            }
            else
            {
                dir = direction;
            }
            foundEnemy.AddForce(dir.normalized * selectedAbility.knockback);
            if (CurrentPierce > 0)
            {
                CurrentPierce--;
            }
        }
    }

    public virtual void PoolSelf()
    {
        ResetAbility();
        anim.Play("Idle");
        Pool.PoolObj(this);
    }
}
