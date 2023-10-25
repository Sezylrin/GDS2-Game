using System;
using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using System.Threading;
using TMPro;
using Random = UnityEngine.Random;

#region EnemyType Enum
public enum EnemyType
{
    TypeError,
    Rhino, 
    Snake,
    Cheetah,
    Lizard,
    Test1, 
    Test2
}
#endregion

public abstract class Enemy : MonoBehaviour, IDamageable, IPoolable<Enemy>
{
    // Enums
    #region EnemyTimer Enum
    protected enum EnemyTimer
    {
        effectedTimer,
        attackCooldownTimer,
        windupDurationTimer,
        attackDurationTimer,
        aiActionTimer,
        attackCD
    }
    #endregion

    // Variables
    #region Enemy Info Variables
    [field: Header("Enemy Info")]
    [field: SerializeField] public EnemyType Type { get; set; }
    [field: SerializeField] public ElementType Element { get; set; } = ElementType.noElement;
    [field: SerializeField] public int Tier { get; set; } = 1;
    [field: SerializeField, ReadOnly] public int Hitpoints { get; set; }
    [field: SerializeField, ReadOnly] protected ElementType ActiveElementEffect { get; set; } = ElementType.noElement;
    [field: SerializeField, ReadOnly] protected int ElementTier { get; set; }
    [field: SerializeField, ReadOnly] protected int CurrentAttack { get; set; }
    [field: SerializeField, ReadOnly] public bool WindingUp { get; private set; }
    [field: SerializeField, ReadOnly] public bool InAttack { get; private set; }
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
    [field: SerializeField, ReadOnly] protected float AttackCooldownDuration { get; set; }

    [field: SerializeField, ReadOnly] protected int Attack1Damage { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack1Duration { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack1Windup { get; set; }
    [field: SerializeField, ReadOnly] protected float AttackKnockback1 { get; set; }
    [field: SerializeField, ReadOnly] protected int Attack2Damage { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack2Duration { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack2Windup { get; set; }
    [field: SerializeField, ReadOnly] protected float AttackKnockback2 { get; set; }
    [field: SerializeField, ReadOnly] protected int Attack3Damage { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack3Duration { get; set; }
    [field: SerializeField, ReadOnly] protected float Attack3Windup { get; set; }
    [field: SerializeField, ReadOnly] protected float AttackKnockback3 { get; set; }

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
    [field: SerializeField] protected Transform PivotPoint { get; set; }
    [field: SerializeField] protected HealthBarSegmentController HealthBarController { get; set; }
    [field: SerializeField] protected StaggerBar StaggerBar { get; set; }
    [field: SerializeField] public Rigidbody2D rb { get; private set; }
    [field: SerializeField] protected Collider2D col2D { get; private set; }
    [field: SerializeField] protected AIPath path { get; set; }
    [SerializeField,Tooltip("What layers the enemy cant walk over or through")]
    protected LayerMask TerrainLayers;
    protected AudioSource WalkingSound { get; set; }
    protected GameObject DeathSoundPrefab { get; set; }
    protected EnemyManager Manager { get; set; }
    protected ElementCombo ComboManager { get; set; }
    protected Vector2 dir { get; set; }
    protected bool hitTarget = false;
    [SerializeField]
    protected float collisionDisabledDur;
    #endregion

    #region Combo Interface Variables
    [field: Header("Combo Interface")]
    [field: SerializeField, ReadOnly] public LayerMask TargetLayer { get; set; }
    [field: SerializeField] protected LayerMask PlayerLayer { get; set; }
    [field: SerializeField] protected LayerMask EnemyLayer { get; set; }
    public bool IsStunned { get; set; }
    [SerializeField]
    protected TMP_Text comboText;
    #endregion

    #region Shader
    [Header("Shaders")]
    [SerializeField]
    private float flashDuration;
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private SpriteRenderer rend;
    [SerializeField]
    private MaterialPropertyBlock block;
    [SerializeField]
    private float[] hues = new float[3];
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
    private float currentBlizzardBonus = 1;
    #endregion

    // Functions
    #region Object Initialization Functions
    public virtual void Init()
    {
        if (block == null)
            block = new MaterialPropertyBlock();
        SetInheritanceSO();
        SetDefaultState();
        if (debugDisableAI) Debug.LogWarning(this + "'s AI is Disabled");
        CancelFlash();
    }

    public virtual void Init(Vector2 spawnLocation, ElementType element, int tier)
    {
        transform.position = spawnLocation;
        Element = element;
        Tier = tier;
        Init();
    }

    private void Awake()
    {
    }
    protected virtual void Start()
    {
        ComboManager = GameManager.Instance.ComboManager;
        Manager = GameManager.Instance.EnemyManager;

        if (debugEnemySpawn) Manager.DebugAddEnemy(this);

        SetTimers();

        path.OnDestinationReached += SetOnDestination;
        Speed = path.maxSpeed;

        ActiveElementEffect = ElementType.noElement;

        Init();

        defaultLayer = col2D.excludeLayers;
    }

    protected virtual void SetInheritanceSO()
    {

    }

    public virtual void SetDefaultState()
    {
        SetStatsFromScriptableObject();
        SetHitPoints();
        SetElementOutline();
        SetHue();

        WindingUp = false;
        Staggered = false;
        AbleToAttack = true;
        Consumable = false;
        ElementTier = 1;
        currentState = EnemyState.stationary;

        targetTr = GameManager.Instance.PlayerTransform;
        TargetLayer = PlayerLayer;

        EnemyTimers.ResetToZero();

        StaggerBar.ResetStagger();
    }

    public virtual void SetStatsFromScriptableObject()
    {
        MaxHealth = SO.maxHealth[Tier - 1];
        Speed = SO.speed[Tier - 1];
        ResetSpeed();
        SetArmour();
        Souls = UnityEngine.Random.Range(SO.minSouls[Tier - 1], SO.maxSouls[Tier - 1] + 1);
        AttackCooldownDuration = SO.attackCooldown[Tier - 1];
        EffectDuration = SO.effectDuration;

        Attack1Damage = SO.attack1Damage[Tier - 1];
        Attack1Duration = SO.attack1Duration;
        Attack1Windup = SO.windup1Duration;
        AttackKnockback1 = SO.attackKnockback1;
        Attack2Damage = SO.attack2Damage[Tier - 1];
        Attack2Duration = SO.attack2Duration;
        Attack2Windup = SO.windup2Duration;
        AttackKnockback2 = SO.attackKnockback2;
        Attack3Damage = SO.attack3Damage[Tier - 1];
        Attack3Duration = SO.attack3Duration;
        Attack3Windup = SO.windup3Duration;
        AttackKnockback3 = SO.attackKnockback3;

        StaggerBar.SetStats(SO.basePointsToStagger, SO.staggerMinDuration, SO.staggerMaxDuration, SO.staggerDelayDuration, SO.staggerDecayAmount, SO.staggerDecayRate, SO.damageToReachMaxDuration);
        HealthBarController.SetStats(MaxHealth, ConsumableHealthPercentThreshold);
    }

    public void SetHitPoints()
    {
        Hitpoints = MaxHealth;
    }

    public void SetOverRideHealth(int amount)
    {
        MaxHealth = amount;
        SetHitPoints();
        HealthBarController.SetStats(MaxHealth, ConsumableHealthPercentThreshold);
    }

    public void SetArmour()
    {
        float floorsComplete = (float)LevelGenerator.Instance.floorsCleared;
        float floorsToFinish = ((float)LevelGenerator.Instance.floorsToWin - 1f) * 2f;
        baseArmour = 1 - (floorsComplete / floorsToFinish);
        //Debug.Log("floorsComplete = " + floorsComplete + ", floorsToFinish = " + floorsToFinish + ", baseArmour = " + baseArmour);
    }

    public void SetTimers()
    {
        EnemyTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(EnemyTimer), gameObject);
        EnemyTimers.times[(int)EnemyTimer.effectedTimer].OnTimeIsZero += RemoveElementEffect;
        EnemyTimers.times[(int)EnemyTimer.attackCooldownTimer].OnTimeIsZero += EndAttackCooldown;
        EnemyTimers.times[(int)EnemyTimer.windupDurationTimer].OnTimeIsZero += EndWindup;
        EnemyTimers.times[(int)EnemyTimer.attackDurationTimer].OnTimeIsZero += EndAttack;

        //ComboEffectTimer = GameManager.Instance.TimerManager.GenerateTimers(typeof(ElementCombos), gameObject);
        /*ComboEffectTimer.times[(int)ElementCombos.aquaVolt].OnTimeIsZero += RemoveStun;
        ComboEffectTimer.times[(int)ElementCombos.noxiousGas].OnTimeIsZero += RemoveNoxious;
        ComboEffectTimer.times[(int)ElementCombos.wither].OnTimeIsZero += RemoveWither;*/
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
            //AttemptAttack();
        }
        if (debugInterruptAttack)
        {
            debugInterruptAttack = false;
            InterruptAttack();
        }
        SetMainTex();
        StateMachine();
        DeterminePathing();
        //EnemyAi();
    }
    private void FixedUpdate()
    {
        ReEnablingPath();
    }
    #endregion

    #region Health Functions
    public virtual void TakeDamage(int damage, int staggerPoints, ElementType type, int tier, ElementType typeTwo = ElementType.noElement)
    {
        if (Hitpoints <= 0) return;
        
        if(currentState == EnemyState.idle) Manager.EnableAggro(); //Makes all enemies on screen aggro'd
        PlayFlash();
        CalculateResist(type, typeTwo);
        float modifier = CalculateModifer();
        float modifiedDamage = damage * modifier;
        Hitpoints -= (int)Mathf.Ceil(modifiedDamage);

        GameManager.Instance.AudioManager.PlaySound(AudioRef.Hit);
        if (Hitpoints <= 0) //Handles death
        {
            Hitpoints = 0;
            InterruptAttack();
            OnDeath();
            return;
        }

        if (typeTwo == ElementType.noElement) 
        {
            ComboManager.AttemptCombo(type, ActiveElementEffect, this, EnemyLayer, CalculateTier(tier, ElementTier), transform.position);
            ApplyElementEffect(type);
        } 
        if (Staggered)
        {
            ElementTier = tier;
            InterruptAttack();
            StaggerBar.IncreaseStaggerDuration((int)Mathf.Ceil(modifiedDamage));
        }

        StaggerBar.AddToStaggerBar(staggerPoints);
        HealthBarController.UpdateSegments(Hitpoints);

        /*
        if (!Consumable)
        {
            if ((float)Hitpoints / (float)MaxHealth * 100 <= ConsumableHealthPercentThreshold)
            {
                ConsumableHitbox.SetActive(true);
                Consumable = true;
            }
        }
        */
    }

    public virtual void TakeDamage(int damage, int staggerPoints, ElementType type, ElementType typeTwo = ElementType.noElement)
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
        return currentStaggerDamage * currentElementResist * baseArmour * currentBlizzardBonus;
    } 

    public void AddForce(Vector2 force)
    {
        if (debugDisableAI)
            return;
        if (currentState == EnemyState.attacking || currentState == EnemyState.chasing || force.magnitude < 0.2f)
            return;
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
        SetOutline(Color.white, 0);
        comboText.text = "";
        if (DeathSoundPrefab) Instantiate(DeathSoundPrefab);
        if (!overrideKill)
        {
            Manager.DecrementActiveEnemyCounter();
            GameManager.Instance.AddSouls(Souls);
        }
        PoolSelf();
    }
    #endregion

    #region Shader
    private Coroutine flash;
    private void PlayFlash()
    {
        block.SetColor("_FlashColour", Color.white);
        if (flash != null)
        {
            StopCoroutine(flash);
            block.SetFloat("_FlashAmount", 0);
            rend.SetPropertyBlock(block);
            flash = StartCoroutine(DamageFlash());
        }
        flash = StartCoroutine(DamageFlash());
    }
    protected void SetOutline(Color colour, float thickness = 1)
    {
        block.SetColor("_OutlineColour", colour);
        block.SetFloat("_Thickness", thickness);
        rend.SetPropertyBlock(block);
    }
    private IEnumerator DamageFlash()
    {
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = curve.Evaluate(elapsedTime / flashDuration);
            block.SetFloat("_FlashAmount", currentFlashAmount);
            rend.SetPropertyBlock(block);
            yield return null;
        }
        block.SetFloat("_FlashAmount", 0);
        rend.SetPropertyBlock(block);
    }

    private void SetMainTex()
    {
        block.SetTexture("_MainTex", rend.sprite.texture);
    }

    protected void CancelFlash()
    {
        block.SetFloat("_FlashAmount", 0);
        rend.SetPropertyBlock(block);
    }

    private IEnumerator WindUpFlash()
    {
        while (WindingUp)
        {
            block.SetColor("_FlashColour", Color.red);
            block.SetFloat("_FlashAmount", 0.65f);
            rend.SetPropertyBlock(block);
            yield return new WaitForSeconds(0.1f);

            block.SetFloat("_FlashAmount", 0);
            rend.SetPropertyBlock(block);
            yield return new WaitForSeconds(0.1f);
        }
        block.SetFloat("_FlashAmount", 0);
        rend.SetPropertyBlock(block);
    }

    private void SetHue()
    {
        block.SetFloat("_HueShift", hues[Tier - 1]);
        rend.SetPropertyBlock(block);
    }
    #endregion

    #region Attacking Functions

    protected virtual int ChooseAttack()
    {        
        return Random.Range(1, Tier+1);
    }

    protected virtual void Attack()
    {
        switch (CurrentAttack)
        {
            case 1:
                BeginWindup(Attack1Windup);
                break;
            case 2:
                BeginWindup(Attack2Windup);
                break;
            case 3:
                BeginWindup(Attack3Windup);
                break;
        }
        AbleToAttack = false;
    }

    protected virtual void BeginAttackCooldown()
    {
        EnemyTimers.SetTime((int)EnemyTimer.attackCooldownTimer, AttackCooldownDuration);
        
    }

    protected virtual void EndAttackCooldown(object sender, EventArgs e)
    {
        AbleToAttack = true;
    }

    protected virtual void BeginWindup(float windupDuration)
    {
        EnemyTimers.SetTime((int)EnemyTimer.windupDurationTimer, windupDuration);
        WindingUp = true;
        StartCoroutine(WindUpFlash());
    }

    protected virtual void EndWindup(object sender, EventArgs e)
    {
        BeginAttack();
        WindingUp = false;
    }

    protected virtual void BeginAttack()
    {
        InAttack = true;
        float duration = 0;
        switch (CurrentAttack)
        {
            case 1:
                duration = Attack1Duration;
                Attack1();
                break;
            case 2:
                duration = Attack2Duration;
                Attack2();
                break;  
            case 3:
                duration = Attack3Duration;
                Attack3();
                break;
        }
        EnemyTimers.SetTime((int)EnemyTimer.attackDurationTimer, duration);
    }

    protected void EndAttack(object sender, EventArgs e)
    {
        //set state to stationary as enemy is done with an attack
        EndAttack();
    }

    protected virtual void EndAttack()
    {
        InAttack = false;
        currentState = EnemyState.stationary;
        BeginAttackCooldown();
        Manager.DoneAttack();
        isAttacking = false;
        hitTarget = false;
    }

    protected virtual void InterruptAttack()
    {
        EnemyTimers.ResetSpecificToZero((int)EnemyTimer.windupDurationTimer);
        EnemyTimers.ResetSpecificToZero((int)EnemyTimer.attackDurationTimer);
        WindingUp = false;
        EnemyTimers.SetTime((int)EnemyTimer.aiActionTimer, 1);
        //interrupted enemy should stop current action
        currentState = EnemyState.stationary;
        if (isAttacking)
            Manager.DoneAttack();
        isAttacking = false;
        hitTarget = false;
    }

    protected virtual void Attack1()
    {

    }
    protected virtual void Attack2()
    {

    }
    protected virtual void Attack3()
    {

    }

    public void DoDamage(IDamageable target)
    {
        switch (CurrentAttack)
        {
            case 1: 
                target.TakeDamage(Attack1Damage, 0, Element);
                target.AddForce(dir.normalized * AttackKnockback1);
                break;
            case 2:
                target.TakeDamage(Attack2Damage, 0, Element);
                target.AddForce(dir.normalized * AttackKnockback2);
                break;
            case 3:
                target.TakeDamage(Attack3Damage, 0, Element);
                target.AddForce(dir.normalized * AttackKnockback3);
                break;
        }
        hitTarget = true;
    }
    #endregion

    #region Element Functions
    protected virtual void ApplyElementEffect(ElementType type)
    {
        EnemyTimers.SetTime((int)EnemyTimer.effectedTimer, EffectDuration);
        ActiveElementEffect = type;
    }

    protected virtual void RemoveElementEffect(object sender, EventArgs e)
    {
        ActiveElementEffect = ElementType.noElement;
    }

    protected virtual void SetElementOutline()
    {
        switch (Element)
        {
            case ElementType.fire:
                SetOutline(Color.red);
                break;
            case ElementType.water:
                SetOutline(Color.blue);
                break;
            case ElementType.electric:
                SetOutline(new Color(0.6950685f, 0.1756853f, 0.7924528f));
                break;
            case ElementType.wind:
                SetOutline(Color.white);
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

    #region Combo Functions

    public void ComboAttack(ComboSO combo, ElementType typeOne, ElementType typeTwo, Color textColour)
    {
        TakeDamage(combo.BaseDamage, combo.StaggerDamage, typeOne, typeTwo);
        comboText.text = combo.name;
        comboText.color = textColour;
        if (gameObject.activeInHierarchy)
            StartCoroutine(RemoveText(combo.name));
    }

    private IEnumerator RemoveText(string textToRemove)
    {
        yield return new WaitForSeconds(2.5f);
        if (comboText.text.Equals(textToRemove))
            comboText.text = "";
    }

    public IEnumerator StunTarget(float dur)
    {
        IsStunned = true;
        StopPathing();
        InterruptAttack();
        yield return new WaitForSeconds(dur);

        IsStunned = false;
    }

    public void InBlizzard(float bonusDamage)
    {
        currentBlizzardBonus = bonusDamage;
    }

    public void ExitBlizzard()
    {
        currentBlizzardBonus = 1;
    }
    #region old
    /*public void ApplyFireSurge(float damage, int Stagger)
    {
        TakeDamage(damage, Stagger, ElementType.fire, ElementType.electric);
        comboText.text = "FireSurge";
        comboText.color = Color.red;
        Invoke("RemoveSurgeText", 1f);
    }

    private void RemoveSurgeText()
    {
        comboText.text = "";
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
        comboText.text = "Stunned";
        comboText.color = Color.yellow;
    }

    private void RemoveStun(object sender, EventArgs e)
    {
        RemoveStun();
    }

    public void RemoveStun()
    {
        IsStunned = false;
        EnemyTimers.SetTime((int)EnemyTimer.aiActionTimer, 0);
        comboText.text = "";
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
        CurrentAttack = ChooseAttack();
        comboText.text = "Noxious";
        comboText.color = Color.magenta;
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
        comboText.text = "";
    }

    public void ApplyWither(float damage, int stagger, float duration, float witherBonus)
    {
        IsWither = true;
        TakeDamage(damage, stagger, ElementType.poison, ElementType.nature);
        duration *= currentElementResist;
        currentWitherBonus = (witherBonus - 1) * currentElementResist + 1;
        ComboEffectTimer.SetTime((int)ElementCombos.noxiousGas, duration);
        comboText.text = "Withered";
        comboText.color = Color.black;
    }

    private void RemoveWither(object sender, EventArgs e)
    {
        RemoveWither();
    }
    public void RemoveWither()
    {
        IsWither = false;
        currentWitherBonus = 1;
        comboText.text = "";
    }*/
    #endregion

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
    [SerializeField, Range(1,10)] protected float RepositionPointMin;
    [SerializeField, Range(1,10)] protected float RepositionPointMax;
    [SerializeField] protected float IdleMoveRateMin;
    [SerializeField] protected float IdleMoveRateMax;
    [SerializeField][ReadOnly]
    protected EnemyState currentState;
    [SerializeField][ReadOnly]
    protected bool hasDestination;
    [SerializeField][ReadOnly]
    protected Transform targetTr;
    [SerializeField] protected bool debugDisableAI = false;
    protected bool isAttacking;
    private Vector2 randDeviate;
    protected virtual void StateMachine()
    {
        if (debugDisableAI) return;
        if (IsStunned || Staggered)
        {
            StopPathing();
            Debug.Log(currentState);
            return;
        }
        if (!hasDestination && currentState.Equals(EnemyState.stationary) && EnemyTimers.IsTimeZero((int)EnemyTimer.aiActionTimer))
        {
            if (AbleToAttack && Manager.CanAttack())
            {
                currentState = EnemyState.chasing;
                CurrentAttack = ChooseAttack();
                isAttacking = true;
            }
            else
            {
                float x = Random.Range(-IdleRadius, IdleRadius);
                float y = Random.Range(-IdleRadius, IdleRadius);
                randDeviate = new Vector2(x, y);
                currentState = EnemyState.repositioning;
                repositionRange = Random.Range(RepositionPointMin, RepositionPointMax);
            }
        }
    }
    protected virtual void DeterminePathing()
    {
        switch ((int)currentState)
        {
            case (int)EnemyState.chasing:
                DetermineAttackPathing();
                break;
            case (int)EnemyState.repositioning:
                RepositionPicker();
                break;
        }
    }
    protected virtual void EnemyAi()
    {
        if (debugDisableAI) return;
        if (IsStunned || Staggered) return;
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
                isAttacking = true;
                CurrentAttack = ChooseAttack();
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
        float x = Random.Range(-IdleRadius, IdleRadius);
        float y = Random.Range(-IdleRadius, IdleRadius);
        SetDestination(new Vector3(x,y,0) + transform.position);
    }

    protected virtual void RepositionPicker()
    {
        BasicReposition();
    }
    private float repositionRange;
    protected void BasicReposition()
    {
        Vector3 targetpoint = targetTr.position;
        Vector3 minimumRange = transform.position - targetpoint;
        minimumRange = minimumRange.normalized * repositionRange;
        targetpoint += minimumRange;
        targetpoint += (Vector3)randDeviate;
        SetDestination(targetpoint);
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
                timeToAdd = Random.Range(IdleMoveRateMin, IdleMoveRateMax);
                break;
            case (int)EnemyState.chasing:
                Attack();
                currentState = EnemyState.attacking;
                switch (CurrentAttack)
                {
                    case 1:
                        timeToAdd = Attack1Duration + Attack1Windup;
                        break;
                    case 2:
                        timeToAdd = Attack2Duration + Attack2Windup;
                        break;
                    case 3:
                        timeToAdd = Attack3Duration + Attack3Windup;
                        break;
                }
                break;
            case (int)EnemyState.repositioning:
                timeToAdd = Random.Range(1, 2f);
                currentState = EnemyState.stationary;
                break;
        }
        if (timeToAdd != 0)
            EnemyTimers.SetTime((int)EnemyTimer.aiActionTimer, timeToAdd);
    }
    protected void SetDestination(Vector3 destination)
    {
        hasDestination = true;
        path.destination = destination;

    }

    public void StopPathing()
    {
        if (GameManager.Instance.IsTutorial)
            return;
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

    #region collisionPrevention
    private float collisionTime = 0;
    private bool collisionOff = false;
    private LayerMask defaultLayer;
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!(collision.collider.CompareTag(Tags.T_Player)|| collision.collider.CompareTag(Tags.T_Terrain)))
        {
            if (collisionTime > 1f && !collisionOff)
            {
                collisionOff = true;
                col2D.excludeLayers += EnemyLayer;
                Invoke("ResumeCollision", collisionDisabledDur);
            }
            collisionTime += Time.deltaTime;
        }
    }

    private void ResumeCollision()
    {
        col2D.excludeLayers = defaultLayer;
        collisionTime = 0;
        collisionOff = false;
    }
    #endregion

    #region Debug
    [ContextMenu("Change State")]
    public void ChangeState()
    {
        currentState = EnemyState.stationary;
    }
    #endregion

    #region Pooling Functions 

    public Pool<Enemy> Pool { get; set; }
    public bool IsPooled { get; set; }
    public virtual void PoolSelf(object sender, EventArgs e)
    {
        PoolSelf();
    }

    public virtual void PoolSelf()
    {
        if (Pool != null)
            Pool.PoolObj(this);
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Animation
    public Vector2 AimAtPlayer()
    {
        return targetTr.position - transform.position;
    }

    public Vector2 NextPathPoint()
    {
        return path.desiredVelocity;
    }
    #endregion
}
