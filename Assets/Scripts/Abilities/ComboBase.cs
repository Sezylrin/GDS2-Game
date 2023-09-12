using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class ComboBase : MonoBehaviour, IPoolable<ComboBase>
{
    protected enum ComboTimers
    {
        lifetime,
        damageTick
    }
    // Start is called before the first frame update
    [SerializeField]
    protected CircleCollider2D col2D;

    [SerializeField]
    protected Timer timer;

    protected LayerMask targetLayer;

    [SerializeField]
    protected List<IDamageable> hitTargets = new List<IDamageable>();

    protected AreaComboSO areaComboSO;

    protected int tier;
    public virtual void Init(AreaComboSO SO, int tier, Vector3 pos, LayerMask target)
    {
        this.tier = tier;
        transform.position = pos;
        areaComboSO = SO;
        col2D.radius = SO.radius;
        targetLayer = target;
        col2D.excludeLayers += ~targetLayer;
        
        timer.SetTime((int)ComboTimers.lifetime, areaComboSO.Duration);
        timer.SetTime((int)ComboTimers.damageTick, 0.1f);
    }
    public void InitSpawn()
    {
        timer = GameManager.Instance.TimerManager.GenerateTimers(typeof(ComboTimers), gameObject);
        timer.times[(int)ComboTimers.lifetime].OnTimeIsZero += PoolSelf;
        timer.times[(int)ComboTimers.damageTick].OnTimeIsZero += DoDamage;
    }

    protected virtual void DoDamage(object sender, EventArgs e)
    {
        timer.SetTime((int)ComboTimers.damageTick, areaComboSO.damageTickRate);
        for (int i = 0; i < hitTargets.Count; i++)
        {
            hitTargets[i].TakeDamage(areaComboSO.BaseDamage[tier], areaComboSO.StaggerDamage, areaComboSO.typeOne, areaComboSO.typeTwo);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (hitTargets.Contains(foundTarget))
            {
                return;
            }
            hitTargets.Add(foundTarget);
        }
        
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        IDamageable foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (!hitTargets.Contains(foundTarget))
            {
                return;
            }
            hitTargets.Remove(foundTarget);
        }
    }

    private void PoolSelf(object sender, EventArgs e)
    {
        PoolSelf();
    }

    #region Pooling
    public Pool<ComboBase> Pool { get; set; }
    public bool IsPooled { get; set; }
    public void PoolSelf()
    {
        timer.ResetToZero();
        col2D.excludeLayers -= ~targetLayer;
        Pool.PoolObj(this);
    }
    #endregion
}
