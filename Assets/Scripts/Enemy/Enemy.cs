using System;
using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using System.Threading;

#region EnemyType Enum
public enum EnemyType
{
    TypeError,
    Cheetah,
    Lizard, 
    Rhino, 
    Snake, 
    Test1, 
    Test2
}
#endregion

public abstract class Enemy : MonoBehaviour, IDamageable, IComboable
{
    // Enums
    #region EnemyTimer Enum
    protected enum EnemyTimer
    {
        effectedTimer,
        attackCooldownTimer,
        windupDurationTimer,
        attackDurationTimer,
        aiActionTimer
    }
    #endregion

    // Variables
    #region Enemy Info Variables
    [field: Header("Enemy Info")]
    [field: SerializeField] protected EnemyType Type { get; set; }
    [field: SerializeField] protected ElementType Element { get; set; } = ElementType.noElement;
    [field: SerializeField, ReadOnly] public int Hitpoints { get; set; }
    [field: SerializeField, ReadOnly] protected ElementType ActiveElementEffect { get; set; }
    [field: SerializeField, ReadOnly] protected int ElementTier { get; set; }
    [field: SerializeField, ReadOnly] protected bool WindingUp { get; set; }
    [field: SerializeField, ReadOnly] protected bool Staggered { get; set; }
    [field: SerializeField, ReadOnly] protected bool AbleToAttack { get; set; }
    [field: SerializeField, ReadOnly] protected bool Consumable { get; set; }
    [field: SerializeField] protected Timer EnemyTimers { get; private set; }
    #endregion

    #region Balance Variables
    [field: Header("Balance Variables")]
    [field: SerializeField, ReadOnly] public int MaxHealth { get; private set; }
    [field: SerializeField, ReadOnly] protected float Speed { get; set; }
    [field: SerializeField, ReadOnly] protected int Souls { get; set; }
    [field: SerializeField, ReadOnly] protected float EffectDuration { get; set; }
    [field: SerializeField, ReadOnly] protected float WindupDuration { get; set; }
    [field: SerializeField, ReadOnly] protected float AttackCooldownDuration { get; set; }

    [field: SerializeField, ReadOnly] protected int Attack1Damage { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack1Duration { get; set; }
    [field: SerializeField, ReadOnly] protected int Attack2Damage { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack2Duration { get; set; }
    [field: SerializeField, ReadOnly] protected int Attack3Damage { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack3Duration { get; set; }
    
    [field: SerializeField, ReadOnly] protected float AttackKnockback { get; set; }
    [field: SerializeField, ReadOnly, Range(0, 100)] protected int ConsumableHealthPercentThreshold { get; set; }
    [field: SerializeField, ReadOnly, Range(0, 100)] protected int HealthPercentReceivedOnConsume { get; set; }
    #endregion

    #region Debug Variables
    [field: Header("Debug Testing")]
    [field: SerializeField] ElementType debugElement { get; set; } = ElementType.water;
    [field: SerializeField] int debugDamage { get; set; } = 20;
    [field: SerializeField] int debugStaggerPoints { get; set; } = 50;
    [field: SerializeField] bool debugTakeDamage { get; set; }
    [field: SerializeField] bool debugApplyElement { get; set; }
    [field: SerializeField] bool debugStartStagger { get; set; }
    [field: SerializeField] bool debugAttemptAttack { get; set; }
    [field: SerializeField] bool debugInterruptAttack { get; set; }
    [field: SerializeField] bool debugEnemySpawn { get; set; } = false;
    #endregion

    #region Inspector Dependent Variables
    [field: Header("Inspector Set Variables")]
    [field: SerializeField] protected BaseEnemyScriptableObject SO { get; set; }
    [field: SerializeField] protected HealthBarSegmentController HealthBarController { get; set; }
    [field: SerializeField] protected StaggerBar StaggerBar { get; set; }
    [field: SerializeField] protected Consume Consume { get; set; }
    [field: SerializeField] protected GameObject ConsumableHitbox { get; set; }
    [field: SerializeField] protected Image ElementEffectImage { get; set; }
    Rigidbody2D IDamageable.rb => rb;
    [field: SerializeField] protected Rigidbody2D rb { get; private set; }
    [field:SerializeField] protected AIPath path { get; set; }
    [field: SerializeField] protected AudioSource WalkingSound { get; set; }
    [field: SerializeField] protected GameObject DeathSoundPrefab { get; set; }
    protected EnemyManager Manager { get; set; }
    protected ElementCombo ComboManager { get; set; }
    #endregion

    #region Combo Interface Variables
    [field: Header("Combo Interface")]
    [field: SerializeField] public List<ElementCombos> ActiveCombos { get; set; }
    [field: SerializeField] public Timer ComboEffectTimer { get; set; }
    [field: SerializeField, ReadOnly] public LayerMask TargetLayer { get; set; }
    [field: SerializeField] protected LayerMask PlayerLayer { get; set; }
    [field: SerializeField] protected LayerMask EnemyLayer { get; set; }
    float IComboable.CurrentWitherBonus { get => currentWitherBonus; set => currentWitherBonus = value; }
    public bool IsNoxious { get; set; }
    public bool IsWither { get; set; }
    public bool IsBrambled { get; set; }
    public bool IsStunned { get; set; }
    #endregion

    #region Damage Modifier Variables
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

    // Functions
    #region Object Initialization Functions
    public virtual void Init()
    {
        SetDefaultState();

        if (debugDisableAI) Debug.LogWarning(this + "'s AI is Disabled");
    }

    public virtual void Init(Vector2 spawnLocation, ElementType element)
    {
        transform.position = spawnLocation;
        Element = element;
        Init();
    }

    protected virtual void Start()
    {
        ComboManager = GameManager.Instance.ComboManager;
        Manager = GameManager.Instance.EnemyManager;

        if (debugEnemySpawn) Manager.DebugAddEnemy(this);

        SetTimers();

        path.OnDestinationReached += SetOnDestination;
        Speed = path.maxSpeed;
    }

    public virtual void SetDefaultState()
    {
        SetStatsFromScriptableObject();
        SetHitPoints();
        SetElementImage();

        WindingUp = false;
        Staggered = false;
        AbleToAttack = true;
        Consumable = false;
        ElementTier = 1;
        currentState = EnemyState.idle;

        ActiveElementEffect = Element;
        spawnPos = transform.position;
        targetTr = GameManager.Instance.PlayerTransform;
        TargetLayer = PlayerLayer;

        EnemyTimers.ResetToZero();

        ConsumableHitbox.SetActive(false);

        StaggerBar.ResetStagger();
    }

    public virtual void SetStatsFromScriptableObject()
    {
        MaxHealth = SO.maxHealth;
        Speed = SO.speed;
        Souls = SO.souls;
        ConsumableHealthPercentThreshold = SO.consumableHealthPercentThreshold;
        HealthPercentReceivedOnConsume = SO.percentToHealOnConsume;
        WindupDuration = SO.windupDuration;
        AttackCooldownDuration = SO.attackCooldown;
        EffectDuration = SO.effectDuration;

        Attack1Damage = SO.attack1Damage;
        Attack1Duration = SO.attack1Duration;
        Attack2Damage = SO.attack2Damage;
        Attack2Duration = SO.attack2Duration;
        Attack3Damage = SO.attack3Damage;
        Attack3Duration = SO.attack3Duration;

        StaggerBar.SetStats(SO.pointsToStagger, SO.staggerDuration, SO.staggerDelayDuration, SO.staggerDecayAmount, SO.staggerDecayRate);
        HealthBarController.SetStats(MaxHealth, ConsumableHealthPercentThreshold);
        Consume.SetStats(HealthPercentReceivedOnConsume);
    }

    public void SetHitPoints()
    {
        Hitpoints = MaxHealth;
    }

    public void SetTimers()
    {
        EnemyTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(EnemyTimer), gameObject);
        EnemyTimers.times[(int)EnemyTimer.effectedTimer].OnTimeIsZero += RemoveElementEffect;
        EnemyTimers.times[(int)EnemyTimer.attackCooldownTimer].OnTimeIsZero += EndAttackCooldown;
        EnemyTimers.times[(int)EnemyTimer.windupDurationTimer].OnTimeIsZero += EndWindup;
        EnemyTimers.times[(int)EnemyTimer.attackDurationTimer].OnTimeIsZero += EndAttack;

        ComboEffectTimer = GameManager.Instance.TimerManager.GenerateTimers(typeof(ElementCombos), gameObject);
        ComboEffectTimer.times[(int)ElementCombos.aquaVolt].OnTimeIsZero += RemoveStun;
        ComboEffectTimer.times[(int)ElementCombos.noxiousGas].OnTimeIsZero += RemoveNoxious;
        ComboEffectTimer.times[(int)ElementCombos.wither].OnTimeIsZero += RemoveWither;
    }

    #endregion

    #region Update Functions
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
        EnemyAi();
    }
    private void FixedUpdate()
    {
        ReEnablingPath();
    }
    #endregion

    #region Health Functions
    public virtual void TakeDamage(float damage, int staggerPoints, ElementType type, int tier, ElementType typeTwo = ElementType.noElement)
    {
        if (Hitpoints <= 0) return;
        
        if(currentState == EnemyState.idle) Manager.EnableAggro(); //Makes all enemies on screen aggro'd

        CalculateResist(type, typeTwo);
        float modifier = CalculateModifer();
        float modifiedDamage = damage * modifier;
        Hitpoints -= (int)Mathf.Ceil(modifiedDamage);
        
        if (Hitpoints <= 0) //Handles death
        {
            Hitpoints = 0;
            OnDeath();
            return;
        }

        if (typeTwo == ElementType.noElement) ComboManager.AttemptCombo(type, ActiveElementEffect, this, EnemyLayer, CalculateTier(tier, ElementTier), transform.position);

        if (typeTwo.Equals(ElementType.noElement) && type != ElementType.noElement)
        {
            ElementTier = tier;
            ApplyElementEffect(type);
            InterruptAttack();
        }

        StaggerBar.AddToStaggerBar(staggerPoints);
        HealthBarController.UpdateSegments(Hitpoints);

        if (!Consumable)
        {
            if (Hitpoints / MaxHealth * 100 <= ConsumableHealthPercentThreshold)
            {
                ConsumableHitbox.SetActive(true);
                Consumable = true;
            }
        }
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

    public void AddForce(Vector2 force)
    {
        rb.velocity = Vector2.zero;
        rb.velocity += force;
        path.enabled = false;
    }

    public void ModifySpeed(float percentage)
    {
        path.maxSpeed = path.maxSpeed * percentage;
    }

    public void ResetSpeed()
    {
        path.maxSpeed = Speed;
    }

    public bool CheckIfConsumable()
    {
        return Consumable;
    }

    public virtual void OnDeath(bool overrideKill = false)
    {
        if (DeathSoundPrefab) Instantiate(DeathSoundPrefab);
        if (!overrideKill)
        {
            Manager.DecrementActiveEnemyCounter();
            GameManager.Instance.AddSouls(Souls);
        }
    }
    #endregion

    #region Attacking Functions
    protected virtual void AttemptAttack()
    {
        if (AbleToAttack && !Staggered) Attack();
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
        EnemyTimers.SetTime((int)EnemyTimer.attackDurationTimer, Attack1Duration);
    }

    protected virtual void EndAttack(object sender, EventArgs e)
    {
        //set state to stationary as enemy is done with an attack
        currentState = EnemyState.stationary;
        EnemyTimers.SetTime((int)EnemyTimer.aiActionTimer, AttackEndAiCD);
    }

    protected virtual void InterruptAttack()
    {
        EnemyTimers.ResetSpecificToZero((int)EnemyTimer.windupDurationTimer);
        WindingUp = false;
        //interrupted enemy should stop current action
        currentState = EnemyState.stationary;
        EnemyTimers.SetTime((int)EnemyTimer.aiActionTimer, AttackEndAiCD);
    }

    #endregion

    #region Element Functions
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

    #region Stagger Functions
    public virtual void BeginStagger()
    {
        Staggered = true;
        currentStaggerDamage = staggerBonusDamage;
    }

    public virtual void EndStagger()
    {
        Staggered = false;
        currentStaggerDamage = 1;
    }
    #endregion

    #region Combo Interface Functions
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
        InterruptAttack();
        StopPathing();
        EnemyTimers.SetTime((int)EnemyTimer.aiActionTimer, float.MaxValue);
    }

    private void RemoveStun(object sender, EventArgs e)
    {
        RemoveStun();
    }

    public void RemoveStun()
    {
        IsStunned = false;
        EnemyTimers.SetTime((int)EnemyTimer.aiActionTimer, 0);
    }

    public void ApplyNoxiousGas(float damage, int stagger, float duration)
    {
        IsNoxious = true;
        TakeDamage(damage, stagger, ElementType.poison, ElementType.wind);
        duration *= currentElementResist;
        ComboEffectTimer.SetTime((int)ElementCombos.noxiousGas, duration);
        //add way to stop attack and stop ai
        TargetLayer = EnemyLayer;
        targetTr = Manager.FindNearestEnemy(transform);
        //ignore manager bucket, instantly attack enemy
        currentState = EnemyState.chasing;
    }

    private void RemoveNoxious(object sender, EventArgs e)
    {
        RemoveNoxious();
    }

    public void RemoveNoxious()
    {
        IsNoxious = false;
        TargetLayer = PlayerLayer;
        targetTr = GameManager.Instance.PlayerTransform;
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

    //AI
    #region AI
    protected enum EnemyState
    {
        idle,
        stationary,
        repositioning,
        chasing,
        attacking
    }

    [Header("AI")]
    [SerializeField, Range(1,10)] protected int IdleRadius;
    [SerializeField, Range(1,10)] protected int RepositionPoint;
    [SerializeField] protected float IdleMoveRateMin;
    [SerializeField] protected float IdleMoveRateMax;
    [SerializeField] protected float RepositionRateMin;
    [SerializeField] protected float RepositionRateMax;
    [SerializeField] protected float AttackEndAiCD;
    [SerializeField][ReadOnly]
    protected EnemyState currentState;
    [SerializeField][ReadOnly]
    protected bool hasDestination;
    private Vector3 spawnPos;
    [SerializeField][ReadOnly]
    protected Transform targetTr;
    [SerializeField] protected bool debugDisableAI = false;

    protected virtual void EnemyAi()
    {
        if (debugDisableAI) return;
        
        if (currentState == EnemyState.chasing)
        {
            DetermineAttackPathing();
            return;
        }
        if (!(EnemyTimers.IsTimeZero((int)EnemyTimer.aiActionTimer) && !hasDestination))
            return;
        if (targetTr == null)
            return;
        if (currentState == EnemyState.stationary)
        {
            if (AbleToAttack && Manager.CanAttack())
            {
                currentState = EnemyState.chasing;
            }
            else
            {
                currentState = EnemyState.repositioning;
            }
        }

        switch ((int)currentState)
        {
            case (int)EnemyState.idle:
                IdlePathPicker();
                break;
            case (int)EnemyState.repositioning:
                RepositionPicker();
                break;
        }

        if(currentState == EnemyState.attacking)
        {
            currentState = EnemyState.stationary;
        }
    }
    //call in child class to determine pathing choice
    protected virtual void DetermineAttackPathing()
    {

    }

    protected virtual void AttackPathPicker()
    {
        SetDestination(targetTr.position);

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
        Vector3 midPoint = targetTr.position - transform.position;
        float distance = midPoint.magnitude - RepositionPoint;
        midPoint = midPoint.normalized * distance;
        while (destination == Vector3.zero)
        {
            float x = UnityEngine.Random.Range(-RepositionPoint * 2, RepositionPoint * 2);
            float y = UnityEngine.Random.Range(-RepositionPoint * 2, RepositionPoint * 2);
            Vector3 point = new Vector3(x, y, 0);
            point += midPoint + transform.position;
            if (!(Vector3.Distance(point, targetTr.position) < RepositionPoint))
            {
                destination = point;
            }
        }
        SetDestination(destination);
    }

    public void SetTarget(Transform targetPos)
    {
        targetTr = targetPos;
    }

    public void BeginAggro()
    {
        currentState = EnemyState.stationary;
    }

    #endregion

    #region Path Finding Functions
    private void SetOnDestination(object sender, EventArgs e)
    {
        StopPathing();
        float timeToAdd = 0;
        switch ((int)currentState)
        {
            case (int)EnemyState.idle:
                timeToAdd = UnityEngine.Random.Range(IdleMoveRateMin, IdleMoveRateMax);
                break;
            case (int)EnemyState.chasing:
                AttemptAttack();
                currentState = EnemyState.attacking;
                timeToAdd = Attack1Duration + WindupDuration;
                break;
            case (int)EnemyState.repositioning:
                timeToAdd = UnityEngine.Random.Range(RepositionRateMin, RepositionRateMax);
                currentState = EnemyState.stationary;
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
        hasDestination = false;
        path.destination = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }

    private void ReEnablingPath()
    {
        if (!path.enabled && rb.velocity.magnitude <= 0.2f)
        {
            path.enabled = true;
        }
    }
    #endregion

    #region Debug
    [ContextMenu("Change State")]
    public void ChangeState()
    {
        currentState = EnemyState.stationary;
    }
    #endregion

    //Obsolete
    #region Pooling Functions (Unused)
    /*
    public Pool<Enemy> Pool { get; set; }
    public bool IsPooled { get; set; }
    public void PoolSelf()
    {
        Pool.PoolObj(this);
    }
    */
    #endregion
}
