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
    protected Transform spriteTR;
    [SerializeField]
    protected List<Enemy> hitTargets = new List<Enemy>();

    protected AreaComboSO areaComboSO;
    public virtual void Init(AreaComboSO SO, Vector3 pos, LayerMask target)
    {
        transform.position = pos;
        areaComboSO = SO;
        col2D.radius = SO.radius;
        col2D.includeLayers = target;
        col2D.excludeLayers = ~target;
        timer.SetTime((int)ComboTimers.lifetime, areaComboSO.duration);
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
            hitTargets[i].TakeDamage(areaComboSO.BaseDamage, areaComboSO.StaggerDamage, areaComboSO.typeOne, areaComboSO.typeTwo);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy foundTarget;
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
        Enemy foundTarget;
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
        col2D.includeLayers = 0;
        col2D.excludeLayers = 0;
        Pool.PoolObj(this);
    }
    #endregion
}
