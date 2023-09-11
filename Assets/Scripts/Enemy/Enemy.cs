using System;
using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using System.Threading;

public enum EnemyType
{
    TypeError,
    Type1,
    Type2, 
    Type3, 
    Type4, 
    Type5, 
    etc
}

public abstract class Enemy : MonoBehaviour, IDamageable, IComboable
{
    protected enum EnemyTimer
    {
        effectedTimer,
        staggerDecayTimer,
        staggerTimer,
        attackCooldownTimer,
        windupDurationTimer,
        attackDurationTimer,
        aiActionTimer
    }

    
    #region Enemy Info
    [field: Header("Enemy Info")]
    [field: SerializeField] protected EnemyType Type { get; set; }
    [field: SerializeField] protected ElementType Element { get; set; } = ElementType.noElement;
    [field: SerializeField] public float Hitpoints { get; set; } = 100;
    [field: SerializeField] protected float MaxHealth { get; set; } = 100;
    [field: SerializeField] protected float Damage { get; set; } = 10;
    [field: SerializeField] protected float Speed { get; set; } = 1;
    [field: SerializeField, ReadOnly] protected int Souls { get; set; } = 1;
    [field: SerializeField, ReadOnly] protected bool WindingUp { get; set; } = false;
    [field: SerializeField] protected Timer EnemyTimers { get; private set; }
    #endregion

    #region Status Effects
    [field: Header("Status Effects")]
    [field: SerializeField, ReadOnly] protected ElementType ActiveElementEffect { get; set; }
    [field: SerializeField, ReadOnly] protected int ElementTier { get; set; }
    [field: SerializeField, ReadOnly] protected float StaggerBar { get; set; }   
    [field: SerializeField, ReadOnly] protected bool Staggered { get; set; } = false;
    [field: SerializeField, ReadOnly] protected bool AbleToAttack { get; set; } = true;
    #endregion

    #region Testing Variables
    [field: Header("Testing Variables")]
    [field: SerializeField] protected float EffectDuration { get; set; } = 5;
    [field: SerializeField] protected float StaggerDecayAmount { get; set; } = 4;
    [field: SerializeField] protected float StaggerDecayRate { get; set; } = 0.25f;
    [field: SerializeField] protected float StaggerDuration { get; set; } = 3;
    [field: SerializeField] protected float AttackDuration { get; set; } = 1;
    [field: SerializeField] protected float AttackCooldownDuration { get; set; } = 10;
    [field: SerializeField] protected int PointsToStagger { get; set; } = 100;
    [field: SerializeField] protected float WindupDuration { get; set; } = 1;

    [field: Header("Debug Testing")]
    [field: SerializeField] ElementType debugElement { get; set; } = ElementType.water;
    [field: SerializeField] int debugDamage { get; set; } = 20;
    [field: SerializeField] int debugStaggerPoints { get; set; } = 50;
    [field: SerializeField] bool debugTakeDamage { get; set; }
    [field: SerializeField] bool debugApplyElement { get; set; }
    [field: SerializeField] bool debugStartStagger { get; set; }
    [field: SerializeField] bool debugAttemptAttack { get; set; }
    [field: SerializeField] bool debugInterruptAttack { get; set; }
    #endregion

    #region Other
    [field: Header("Other")]
    [field: SerializeField] protected AudioSource WalkingSound { get; set; }
    [field: SerializeField] protected GameObject DeathSoundPrefab { get; set; }
    [field: SerializeField] protected Image HealthBarImage { get; set; }
    protected float HealthBarPercentage { get; set; }
    [field: SerializeField] protected Image ElementEffectImage { get; set; }
    [field: SerializeField] protected GameObject StaggeredImage { get; set; }
    Rigidbody2D IDamageable.rb => rb;
    [field: SerializeField] protected Rigidbody2D rb { get; private set; }
    [field:SerializeField] protected AIPath path { get; set; }
    protected EnemyManager Manager { get; set; }
    #endregion

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

    #region DamageModifiers
    [Header("DamageModifiers")]
    [SerializeField, Range(1,3)]
    private float staggerBonusDamage;
    private float currentStaggerDamage = 1;
    [SerializeField, Range(0.1f,1)]
    private float elementResistAmount;
    private float currentElementResist = 1;
    [SerializeField, Range(0.5f, 1)]
    private float baseArmour = 0.5f;
    private float currentWitherBonus = 1;
    #endregion

    //protected EnemyManager Manager { get; set; }
    //protected Player Player { get; set; }

    public virtual void Init()
    {
        SetStats();
        ActiveElementEffect = Element;
        ElementTier = 1;
        spawnPos = transform.position;
        currentState = EnemyState.idle;
        EnemyAi();
        SetElementImage();
    }

    public virtual void Init(Vector2 spawnLocation, ElementType element)
    {
        transform.position = spawnLocation;
        Element = element;
        Init();
    }

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Start()
    {
        Manager = GameManager.Instance.EnemyManager;

        SetTimers();
        EnemyTimers = TimerManager.Instance.GenerateTimers(typeof(EnemyTimer), gameObject);
        EnemyTimers.times[(int)EnemyTimer.effectedTimer].OnTimeIsZero += RemoveElementEffect;
        EnemyTimers.times[(int)EnemyTimer.staggerDecayTimer].OnTimeIsZero += DecrementStaggerBar;
        EnemyTimers.times[(int)EnemyTimer.staggerTimer].OnTimeIsZero += EndStagger;
        EnemyTimers.times[(int)EnemyTimer.attackCooldownTimer].OnTimeIsZero += EndAttackCooldown;
        EnemyTimers.times[(int)EnemyTimer.windupDurationTimer].OnTimeIsZero += EndWindup;
        EnemyTimers.times[(int)EnemyTimer.attackDurationTimer].OnTimeIsZero += EndAttack;
        EnemyTimers.times[(int)EnemyTimer.aiActionTimer].OnTimeIsZero += CallAI;
        StartStaggerDecayTimer();

        path.OnDestinationReached += SetOnDestination;
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
        if (debugInterruptAttack)
        {
            debugInterruptAttack = false;
            InterruptAttack();
        }
    }

    public virtual void SetStats()
    {
        SetHitPoints();
        if (HealthBarImage) HealthBarImage.fillAmount = 100;
    }

    public void SetHitPoints()
    {
        Hitpoints = MaxHealth;
    }

    #region DamageFunctions
    public virtual void TakeDamage(float damage, int staggerPoints, ElementType type, int tier, ElementType typeTwo = ElementType.noElement)
    {
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
        AddToStaggerBar(staggerPoints);

        HealthBarPercentage = Hitpoints / MaxHealth;
        if (HealthBarImage) HealthBarImage.fillAmount = HealthBarPercentage;

        if (WindingUp) InterruptAttack();
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
        Manager.DecrementActiveEnemyCounter();
        GameManager.Instance.AddSouls(Souls);
    }

    public void AddForce(Vector2 force)
    {
        rb.velocity += force;
    }
    #endregion

    #region Attacking
    protected virtual void AttemptAttack() //Check if Enemy Manager has an attack point available
    {
        if (Manager.CanAttack() && AbleToAttack && !Staggered) Attack();
    }

    protected virtual void Attack()
    {
        BeginAttackCooldown();
        BeginWindup();
        Manager.ManagerAttack();
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

    protected virtual void BeginWindup()
    {
        EnemyTimers.SetTime((int)EnemyTimer.windupDurationTimer, WindupDuration);
        WindingUp = true;
    }

    protected virtual void EndWindup(object sender, EventArgs e)
    {
        BeginAttack();
        WindingUp = false;
    }

    protected virtual void BeginAttack()
    {
        EnemyTimers.SetTime((int)EnemyTimer.attackDurationTimer, AttackDuration);
    }

    protected virtual void EndAttack(object sender, EventArgs e)
    {

    }

    protected virtual void InterruptAttack()
    {
        EnemyTimers.ResetSpecificToZero((int)EnemyTimer.windupDurationTimer);
        WindingUp = false;
    }

    #endregion

    #region Element
    protected virtual void ApplyElementEffect(ElementType type)
    {
        EnemyTimers.SetTime((int)EnemyTimer.effectedTimer, EffectDuration);
        ActiveElementEffect = type;
        SetElementImage();
    }

    protected virtual void RemoveElementEffect(object sender, EventArgs e)
    {
        ActiveElementEffect = Element;
        SetElementImage();
    }

    protected virtual void SetElementImage()
    {
        switch (ActiveElementEffect)
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
        currentStaggerDamage = staggerBonusDamage;
        StaggeredImage.SetActive(true);
    }

    protected virtual void EndStagger(object sender, EventArgs e)
    {
        Staggered = false;
        currentStaggerDamage = 1;
        StaggeredImage.SetActive(false);
    }

    protected virtual void StartStaggerDecayTimer()
    {
        EnemyTimers.SetTime((int)EnemyTimer.staggerDecayTimer, StaggerDecayRate);
    }

    protected virtual void DecrementStaggerBar(object sender, EventArgs e)
    {
        if (StaggerBar > 0)
        {
            StaggerBar -= StaggerDecayAmount;
            StartStaggerDecayTimer();
        }  
    }

    protected virtual void AddToStaggerBar(int staggerPoints)
    {
        if (StaggerBar <= 0) StartStaggerDecayTimer();
        StaggerBar += staggerPoints;

        if (StaggerBar >= PointsToStagger)
        {
            BeginStagger();
            StaggerBar = 0;
        }
    }
    #endregion

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

    #region AI
    protected enum EnemyState
    {
        idle,
        stationary,
        repositioning,
        chasing,
        attacking
    }

    [field: Header("AI")]
    [field: SerializeField, Range(1,10)] protected int IdleRadius;
    [field: SerializeField, Range(1,10)] protected int RepositionPoint;
    [field: SerializeField] protected float IdleMoveRateMin;
    [field: SerializeField] protected float IdleMoveRateMax;
    [field: SerializeField] protected float RepositionRateMin;
    [field: SerializeField] protected float RepositionRateMax;

    protected EnemyState currentState;
    protected bool hasDestination;
    private Vector3 spawnPos;
    private Vector3 targetPos;
    private void DestinationChecker()
    {
        if (hasDestination)
            return;
        switch ((int)currentState)
        {
            case (int)EnemyState.idle:
                IdlePathPicker();
                break;
        }
    }

    protected virtual void EnemyAi()
    {
        if (currentState == EnemyState.stationary)
        {

        }

        switch ((int)currentState)
        {
            case (int)EnemyState.idle:
                IdlePathPicker();
                break;
            case (int)EnemyState.repositioning:
                break;
        }
    }

    private void CallAI(object sender, EventArgs e)
    {
        EnemyAi();
    }

    protected virtual void AttackPathPicker()
    {
        SetDestination(targetPos);

    }

    protected virtual void IdlePathPicker()
    {
        float x = UnityEngine.Random.Range(-IdleRadius, IdleRadius);
        float y = UnityEngine.Random.Range(-IdleRadius, IdleRadius);
        SetDestination(new Vector3(x,y,0) + spawnPos);
    }

    protected virtual void RepositionPicker()
    {
        Vector3 destination = Vector3.zero;
        Vector3 midPoint = targetPos - transform.position;
        float distance = midPoint.magnitude - RepositionPoint;
        midPoint = midPoint.normalized * distance;
        while (destination == Vector3.zero)
        {
            float x = UnityEngine.Random.Range(-RepositionPoint * 2, RepositionPoint * 2);
            float y = UnityEngine.Random.Range(-RepositionPoint * 2, RepositionPoint * 2);
            Vector3 point = new Vector3(x, y, 0);
            point += midPoint + transform.position;
            if (!(Vector3.Distance(point, targetPos) < RepositionPoint))
            {
                destination = point;
            }
        }
        SetDestination(destination);
    }

    public void SetTarget(Vector3 targetPos)
    {
        this.targetPos = targetPos; 
    }

    #endregion

    #region PathFinding
    private void SetOnDestination(object sender, EventArgs e)
    {
        hasDestination = false;
        float timeToAdd = 0;
        switch ((int)currentState)
        {
            case (int)EnemyState.idle:
                timeToAdd = UnityEngine.Random.Range(IdleMoveRateMin, IdleMoveRateMax);
                break;
        }
        EnemyTimers.SetTime((int)EnemyTimer.aiActionTimer, timeToAdd);
    }
    protected void SetDestination(Vector3 destination)
    {
        hasDestination = true;
        path.destination = destination;

    }

    protected void StopPathing()
    {
        path.destination = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }
    #endregion
}
