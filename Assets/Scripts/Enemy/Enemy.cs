using System;
using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    protected enum EnemyTimer
    {
        effectedTimer,
        staggerTimer,
        attackCooldownTimer,
        windupDurationTimer,
        attackDurationTimer,
        staggerScale
    }

    [field: Header("Enemy Info")]
    [field: SerializeField] protected EnemyType Type { get; set; }
    [field: SerializeField] protected ElementType Element { get; set; } = ElementType.noElement;
    [field: SerializeField] public float Hitpoints { get; set; } = 100;
    [field: SerializeField] protected float MaxHealth { get; set; } = 100;
    [field: SerializeField] protected float Damage { get; set; } = 10;
    [field: SerializeField] protected float Speed { get; set; } = 1;
    [field: SerializeField, ReadOnly] protected float Souls { get; set; } = 1;
    [field: SerializeField] protected Timer EnemyTimers { get; private set; }

    [field: Header("Status Effects")]
    [field: SerializeField, ReadOnly] protected ElementType ActiveElementEffect { get; set; }
    [field: SerializeField, ReadOnly] protected bool Staggered { get; set; } = false;
    [field: SerializeField, ReadOnly] protected bool AbleToAttack { get; set; } = true;

    [field: Header("Testing Variables")]
    [field: SerializeField] protected float EffectDuration { get; set; } = 5;
    [field: SerializeField] protected float StaggerDuration { get; set; } = 3;
    [field: SerializeField] protected float AttackDuration { get; set; } = 1;
    [field: SerializeField] protected float AttackCooldownDuration { get; set; } = 10;
    [field: SerializeField] protected int PointsToStagger { get; set; } = 100;
    [field: SerializeField] protected float WindupDuration { get; set; } = 1;
    [field: SerializeField] ElementType debugElement { get; set; } = ElementType.water;
    [field: SerializeField] int debugDamage { get; set; } = 20;
    [field: SerializeField] int debugStaggerPoints { get; set; } = 50;
    [field: SerializeField] bool debugTakeDamage { get; set; }
    [field: SerializeField] bool debugApplyElement { get; set; }
    [field: SerializeField] bool debugStartStagger { get; set; }
    [field: SerializeField] bool debugAttemptAttack { get; set; }

    [field: Header("Other")]
    [field: SerializeField] protected AudioSource WalkingSound { get; set; }
    [field: SerializeField] protected GameObject DeathSoundPrefab { get; set; }
    [field: SerializeField] protected Image HealthBarImage { get; set; }
    protected float HealthBarPercentage { get; set; }
    [field: SerializeField] protected Image ElementEffectImage { get; set; }
    [field: SerializeField] protected GameObject StaggeredImage { get; set; }

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


    //protected EnemyManager Manager { get; set; }
    //protected Player Player { get; set; }

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
        SetTimers();
        EnemyTimers = TimerManager.Instance.GenerateTimers(typeof(EnemyTimer), gameObject);
        EnemyTimers.times[(int)EnemyTimer.effectedTimer].OnTimeIsZero += RemoveElementEffect;
        EnemyTimers.times[(int)EnemyTimer.staggerTimer].OnTimeIsZero += EndStagger;
        EnemyTimers.times[(int)EnemyTimer.attackCooldownTimer].OnTimeIsZero += EndAttackCooldown;
        EnemyTimers.times[(int)EnemyTimer.windupDurationTimer].OnTimeIsZero += EndWindup;
        EnemyTimers.times[(int)EnemyTimer.attackDurationTimer].OnTimeIsZero += EndAttack;
    }

    protected virtual void Update()
    {
        if (debugApplyElement)
        {
            debugApplyElement = false;
            ApplyElementEffect(debugElement);
        }
        if (debugStartStagger)
        {
            debugStartStagger = false;
            BeginStagger();
        }
        if (debugTakeDamage)
        {
            debugTakeDamage = false;
            TakeDamage(debugDamage, debugElement, debugStaggerPoints);
        }
        if (debugAttemptAttack)
        {
            debugAttemptAttack = false;
            AttemptAttack();
        }
    }

    public virtual void SetStats()
    {
        SetHitPoints();
    }

    public void SetHitPoints()
    {
        Hitpoints = MaxHealth;
    }

    public virtual void TakeDamage(float damage, ElementType type, int staggerPoints)
    {
        /* if (CheckCombo() || CheckResistance()) CurrentHealth -= damage * damageMultiplier;
        else */ Hitpoints -= damage;
        if (Hitpoints <= 0)
        {
            OnDeath();
            return;
        }
         
        ApplyElementEffect(type);
        AddToStaggerScale(staggerPoints);

        HealthBarPercentage = Hitpoints / MaxHealth;
        if (HealthBarImage) HealthBarImage.fillAmount = HealthBarPercentage;
    }

    public virtual void OnDeath()
    {
        if (DeathSoundPrefab) Instantiate(DeathSoundPrefab);
        poolSelf();
    }

    protected virtual void AttemptAttack() //Check if Enemy Manager has an attack point available
    {
        //if (Manager.CanAttack && AbleToAttack)
        if (AbleToAttack)
            Attack();
    }

    protected virtual void Attack()
    {
        BeginAttackCooldown();
        BeginWindup();
    }

    protected virtual void ApplyElementEffect(ElementType type)
    {
        EnemyTimers.SetTime((int)EnemyTimer.effectedTimer, EffectDuration);
        ActiveElementEffect = type;
        switch (type)
        {
            case ElementType.noElement:
                ElementEffectImage.color = Color.grey;
                break;
            case ElementType.fire:
                ElementEffectImage.color = Color.red;
                break;
            case ElementType.water:
                ElementEffectImage.color = Color.blue;
                break;
            case ElementType.electric:
                ElementEffectImage.color = Color.yellow;
                break;
            case ElementType.wind:
                ElementEffectImage.color = Color.white;
                break;
            case ElementType.poison:
                ElementEffectImage.color = Color.magenta;
                break;
            case ElementType.nature:
                ElementEffectImage.color = Color.green;
                break;
        }
    }

    protected virtual void RemoveElementEffect(object sender, EventArgs e)
    {
        ActiveElementEffect = Element;
        switch (Element)
        {
            case ElementType.noElement:
                ElementEffectImage.color = Color.grey;
                break;
            case ElementType.fire:
                ElementEffectImage.color = Color.red;
                break;
            case ElementType.water:
                ElementEffectImage.color = Color.blue;
                break;
            case ElementType.electric:
                ElementEffectImage.color = Color.yellow;
                break;
            case ElementType.wind:
                ElementEffectImage.color = Color.white;
                break;
            case ElementType.poison:
                ElementEffectImage.color = Color.magenta;
                break;
            case ElementType.nature:
                ElementEffectImage.color = Color.green;
                break;
        }
    }

    protected virtual void BeginStagger()
    {
        EnemyTimers.SetTime((int)EnemyTimer.staggerTimer, StaggerDuration);
        Staggered = true;
        StaggeredImage.SetActive(true);
    }

    protected virtual void EndStagger(object sender, EventArgs e)
    {
        Staggered = false;
        StaggeredImage.SetActive(false);
    }

    protected virtual void BeginAttackCooldown()
    {
        EnemyTimers.SetTime((int)EnemyTimer.attackCooldownTimer, AttackCooldownDuration);
        AbleToAttack = false;
    }
    
    protected virtual void EndAttackCooldown(object sender, EventArgs e)
    {
        AbleToAttack = true;
    }

    protected virtual void AddToStaggerScale(int staggerPoints)
    {
        EnemyTimers.ReduceCoolDown((int)EnemyTimer.staggerScale, staggerPoints / -10);

        if (EnemyTimers.GetTime((int)EnemyTimer.staggerScale) * 10 >= PointsToStagger)
        {
            EnemyTimers.SetTime((int)EnemyTimer.staggerScale, 0);
            BeginStagger();
        }
    }

    protected virtual void BeginWindup()
    {
        EnemyTimers.SetTime((int)EnemyTimer.windupDurationTimer, WindupDuration);
    }

    protected virtual void EndWindup(object sender, EventArgs e)
    {
        BeginAttack();
    }

    protected virtual void BeginAttack()
    {
        EnemyTimers.SetTime((int)EnemyTimer.attackDurationTimer, AttackDuration);
    }

    protected virtual void EndAttack(object sender, EventArgs e)
    {

    }

    #region Combo Interface Methods
    public void SetTimers()
    {
        ComboEffectTimer = TimerManager.Instance.GenerateTimers(typeof(ElementCombos), gameObject);
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
