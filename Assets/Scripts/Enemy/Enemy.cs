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
    [field: SerializeField] protected ElementType ActiveElementEffect { get; set; }
    [field: SerializeField] protected int ElementTier { get; set; }
    [field: SerializeField] protected bool Staggered { get; set; } = false;
    [field: SerializeField] protected bool AbleToAttack { get; set; } = true;

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
    Rigidbody2D IDamageable.rb => rb;
    [field: SerializeField] protected Rigidbody2D rb { get; private set; }

    #region Combo Interface Properties
    [field: Header("Combo Interface")]
    [field: SerializeField] public Transform SpawnPosition { get; set; }
    [field: SerializeField] public List<ElementCombos> ActiveCombos { get; set; }
    [field: SerializeField] public Timer ComboEffectTimer { get; set; }
    [field: SerializeField] public LayerMask TargetLayer { get; set; }
    float IComboable.CurrentWitherBonus { get => currentWitherBonus; set => currentWitherBonus = value; }
    public bool IsNoxious { get; set; }
    public bool IsWither { get; set; }
    public bool IsBrambled { get; set; }
    public bool IsStunned { get; set; }
    #endregion

    #region Pooling
    public Pool<Enemy> Pool { get; set; }
    public bool IsPooled { get; set; }

    #endregion

    #region DamageModifiers
    [Header("DamageModifiers")]
    [SerializeField, Range(1,3)]
    private float staggerBonusDamage;
    private float currentStaggerDamage = 1;
    [SerializeField, Range(0.1f,1)]
    private float elementResistAmount;
    private float currentElementResist = 1;
    [SerializeField, Range(0.5f, 1)]
    private float baseArmour;
    private float currentWitherBonus = 1;
    #endregion
    //protected EnemyManager Manager { get; set; }
    //protected Player Player { get; set; }

    protected virtual void Init()
    {
        //Manager = GameManager.EnemyManager;
        SetStats();
        ActiveElementEffect = Element;
        ElementTier = 1;
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
            TakeDamage(debugDamage, debugStaggerPoints, debugElement);
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
    #region DamageFunctions
    public virtual void TakeDamage(float damage, int staggerPoints, ElementType type, int tier, ElementType typeTwo = ElementType.noElement)
    {
        /* if (CheckCombo() || CheckResistance()) CurrentHealth -= damage * damageMultiplier;
        else */
        CalculateResist(type, typeTwo);
        float modifier = CalculateModifer();
        float modifiedDamage = damage * modifier;
        Hitpoints -= modifiedDamage;

        ElementCombo.Instance.AttemptCombo(type, ActiveElementEffect, this, gameObject.layer, CalculateTier(tier, ElementTier), transform.position);

        if (Hitpoints <= 0)
        {
            Hitpoints = 0;
            OnDeath();
            return;
        }
        if (typeTwo.Equals(ElementType.noElement))
        {
            ElementTier = tier;
            ApplyElementEffect(type);
        }
        AddToStaggerScale(staggerPoints);

        HealthBarPercentage = Hitpoints / MaxHealth;
        if (HealthBarImage) HealthBarImage.fillAmount = HealthBarPercentage;
    }
    public virtual void TakeDamage(float damage, int staggerPoints, ElementType type, ElementType typeTwo = ElementType.noElement)
    {
        TakeDamage(damage, staggerPoints, type, 0, typeTwo);
    }

    private int CalculateTier(int a, int b)
    {
        int average = a + b;
        average = Mathf.FloorToInt(average * 0.5f);
        return average - 1;
    }

    private void CalculateResist(ElementType type, ElementType typeTwo = ElementType.noElement)
    {
        if (type.Equals(Type) || (typeTwo.Equals(Type) && !typeTwo.Equals(ElementType.noElement)))
            currentElementResist = elementResistAmount;
        else
            currentElementResist = 1;
    }
    protected float CalculateModifer()
    {
        return currentStaggerDamage * currentElementResist * baseArmour * currentWitherBonus;
    } 

    public virtual void OnDeath()
    {
        if (DeathSoundPrefab) Instantiate(DeathSoundPrefab);
        PoolSelf();
    }

    public void AddForce(Vector2 force)
    {
        rb.velocity += force;
    }
    #endregion

    #region Attacking
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

    protected virtual void BeginAttackCooldown()
    {
        EnemyTimers.SetTime((int)EnemyTimer.attackCooldownTimer, AttackCooldownDuration);
        AbleToAttack = false;
    }

    protected virtual void EndAttackCooldown(object sender, EventArgs e)
    {
        AbleToAttack = true;
    }
    #endregion

    #region Element
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
    #endregion

    #region Stagger
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



    protected virtual void AddToStaggerScale(int staggerPoints)
    {
        EnemyTimers.ReduceCoolDown((int)EnemyTimer.staggerScale, staggerPoints / -10);

        if (EnemyTimers.GetTime((int)EnemyTimer.staggerScale) * 10 >= PointsToStagger)
        {
            EnemyTimers.SetTime((int)EnemyTimer.staggerScale, 0);
            BeginStagger();
        }
    }
    #endregion

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
        ComboEffectTimer.times[(int)ElementCombos.aquaVolt].OnTimeIsZero += RemoveStun;
        ComboEffectTimer.times[(int)ElementCombos.noxiousGas].OnTimeIsZero += RemoveNoxious;
        ComboEffectTimer.times[(int)ElementCombos.wither].OnTimeIsZero += RemoveWither;
    }

    public void ApplyFireSurge(float damage, int Stagger)
    {
        TakeDamage(damage, Stagger, ElementType.fire, ElementType.electric);
    }

    public void ApplyAquaVolt(float damage, int stagger, float duration)
    {
        IsStunned = true;
        TakeDamage(damage, stagger, ElementType.electric, ElementType.water);
        duration *= currentElementResist;
        ComboEffectTimer.SetTime((int)ElementCombos.aquaVolt, duration);
        //add way to stop attack and stop ai
    }

    private void RemoveStun(object sender, EventArgs e)
    {
        RemoveStun();
    }

    public void RemoveStun()
    {
        IsStunned = false;
    }

    public void ApplyNoxiousGas(float damage, int stagger, float duration)
    {
        IsNoxious = true;
        TakeDamage(damage, stagger, ElementType.poison, ElementType.wind);
        duration *= currentElementResist;
        ComboEffectTimer.SetTime((int)ElementCombos.noxiousGas, duration);
        //add way to stop attack and stop ai

    }

    private void RemoveNoxious(object sender, EventArgs e)
    {
        RemoveNoxious();
    }

    public void RemoveNoxious()
    {
        IsNoxious = false;
    }

    public void ApplyWither(float damage, int stagger, float duration, float witherBonus)
    {
        IsWither = true;
        TakeDamage(damage, stagger, ElementType.poison, ElementType.nature);
        duration *= currentElementResist;
        currentWitherBonus = (witherBonus - 1) * currentElementResist + 1;
        ComboEffectTimer.SetTime((int)ElementCombos.noxiousGas, duration);
    }

    private void RemoveWither(object sender, EventArgs e)
    {
        RemoveWither();
    }
    public void RemoveWither()
    {
        IsWither = false;
        currentWitherBonus = 1;
    }
    #endregion

    #region Pooling
    public void PoolSelf()
    {
        Pool.PoolObj(this);
    }
    #endregion
}
