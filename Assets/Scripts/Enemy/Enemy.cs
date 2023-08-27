using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Type1,
    Type2, 
    Type3, 
    Type4, 
    Type5, 
    etc
}


public abstract class Enemy : MonoBehaviour, IDamageable, IComboable, IPoolable<Enemy>
{
    protected enum EnemyTimers
    {
        effectedTimer
    }

    [field: Header("Enemy Info")]
    [field: SerializeField] protected EnemyType Type { get; set; }
    [field: SerializeField] protected ElementType Element { get; set; } = ElementType.noElement;
    [field: SerializeField] public float Hitpoints { get; set; }
    [field: SerializeField] protected float MaxHealth { get; set; } 
    [field: SerializeField] protected float Damage { get; set; }
    [field: SerializeField] protected float Speed { get; set; }
    [field: SerializeField, ReadOnly] protected float Souls { get; set; }
    [field: SerializeField] protected ElementType ActiveElementEffect { get; set; }

    [field: Header("Other")]
    [field: SerializeField] protected AudioSource WalkingSound { get; set; }
    [field: SerializeField] protected AudioSource DeathSound { get; set; }
    [field: SerializeField] protected Timer timer { get; private set; }

    [field: Header("Testing Variables")]
    [field: SerializeField] protected int EffectDuration { get; set; } = 5;

    #region Combo Interface Properties
    [field: Header("Combo Interface")]
    [field: SerializeField] public Transform SpawnPosition { get; set; }
    [field: SerializeField] public List<ElementCombos> ActiveCombos { get; set; }
    [field: SerializeField] public Timer ComboEffectTimer { get; set; }
    [field: SerializeField] public LayerMask TargetLayer { get; set; }
    #endregion

    #region Pooling
    public Pool<Enemy> Pool { get; set; }
    public bool IsPooled { get; set; }
    #endregion


   
    //public EnemyManager Manager { get; set; }
    //punlic Player Player { get; set; }

    protected virtual void Init()
    {
        //Manager = GameManager.EnemyManager;
        SetStats();
        ActiveElementEffect = Element;

    }

    protected virtual void Awake()
    {
        Init();

    }

    protected virtual void Start()
    {
        timer = TimerManager.Instance.GenerateTimers(typeof(EnemyTimers), gameObject);
        timer.OnTimeIsZero += RemoveElementEffect;
    }

    public virtual void SetStats()
    {

        SetHitPoints();
    }

    public void SetHitPoints()
    {
        Hitpoints = MaxHealth;
    }

    public virtual void TakeDamage(float damage, ElementType type)
    {
        /* if (CheckCombo() || CheckResistance())
        {
            CurrentHealth -= damage * damageMultiplier;
        }
        else */ Hitpoints -= damage;
        if (Hitpoints <= 0) OnDeath();
        ApplyElementEffect(type);
    }

    public virtual void OnDeath()
    {
        if (DeathSound) DeathSound.Play();
        poolSelf();
    }

    protected virtual void AttemptAttack() //Check if Enemy Manager has an attack point available
    {
        //if (Manager.CanAttack) Attack()
    }

    protected virtual void Attack()
    {
       //Attacking logic
    }

    protected virtual void ApplyElementEffect(ElementType type)
    {
        timer.SetTime((int)EnemyTimers.effectedTimer, EffectDuration);
        ActiveElementEffect = type;
    }

    protected virtual void RemoveElementEffect(object sender, Timer.OnTimeIsZeroEventArgs e)
    {
        if (e.timerSlot == (int)EnemyTimers.effectedTimer)
            ActiveElementEffect = Element;
    }



    #region Combo Interface Methods
    public void SetTimers()
    {

    }

    public void ApplyFireSurge()
    {

    }

    public void ApplyAquaVolt()
    {

    }

    public void ApplyFireTornado(LayerMask Target)
    {

    }

    public void ApplyBrambles(LayerMask Target)
    {
        
    }

    public void ApplyNoxiousGas()
    {
        
    }

    public void ApplyWither()
    {
        
    }
    #endregion

    #region Pooling
    public void poolSelf()
    {
        Pool.PoolObj(this);
    }
    #endregion
}
