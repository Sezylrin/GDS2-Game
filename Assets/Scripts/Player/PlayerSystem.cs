using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using System;
using UnityEngine.InputSystem;
public class PlayerSystem : MonoBehaviour, IDamageable
{
    private enum SystemCD
    {
        pointRegenDelay,
        iFrames,
        counterAttackQTE
    }

    [Header("General")]
    [SerializeField]
    private Timer timer;
    [SerializeField]
    private PlayerComponentManager PCM;
    [SerializeField]
    private float iframes;

    private void Start()
    {
        SetHitPoints();
        consumeBar = 0;
        canConsume = false;
        timer = GameManager.Instance.TimerManager.GenerateTimers(typeof(SystemCD), gameObject);
        timer.times[(int)SystemCD.counterAttackQTE].OnTimeIsZero += RemoveCounterQTE;
        InitCastPoints();
    }

    #region Update
    private void Update()
    {
        RegeneratePoints();
    }
    #endregion

    #region Ability
    [Header("Ability Stats")]
    [SerializeField]
    private int MaxCastPoints;
    [SerializeField]
    [ReadOnly]
    private int currentCastPoints;
    [ReadOnly, SerializeField]
    private float pointRegenRate;
    [SerializeField]
    private float maxRegenTime;
    [SerializeField]
    [ReadOnly]
    private float regenTimer;
    [SerializeField]
    private float regenDelay;
    [SerializeField]
    private int debugCost;
    private int CurrentCastPoints
    {
        get { return currentCastPoints; }
        set
        {
            if (CurrentCastPoints != value)
            {
                currentCastPoints = value;
                PCM.UI.UpdateSKillPointUI(value);
            }
        }
    }

    [ContextMenu("attempt cast")]
    public void AttemptCastDbug()
    {
        AttemptCast(debugCost);
    }
    public bool AttemptCast(int cost)
    {
        if (cost <= CurrentCastPoints)
        {
            timer.SetTime((int)SystemCD.pointRegenDelay, regenDelay);
            regenTimer -= cost * pointRegenRate;
            CurrentCastPoints -= cost;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanCast(int cost)
    {
        if (cost <= CurrentCastPoints)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RegeneratePoints()
    {
        if (!timer.IsTimeZero((int)SystemCD.pointRegenDelay))
            return;
        if (regenTimer < maxRegenTime)
        {
            CalculatePoints();
            regenTimer += Time.deltaTime;
        }
        else if (regenTimer > maxRegenTime)
        {
            CurrentCastPoints = MaxCastPoints;
            regenTimer = maxRegenTime;
        }
    }
    private void InitCastPoints()
    {
        for (int i = 0; i < MaxCastPoints; i++)
        {
            PCM.UI.AddMoreSkillPoint();
        }
        UpdateCastPoints();
    }
    private void UpdateCastPoints()
    {
        pointRegenRate = maxRegenTime / MaxCastPoints;
        CurrentCastPoints = MaxCastPoints;
        regenTimer = maxRegenTime;
    }

    public void AddCastPoint()
    {
        MaxCastPoints++;
        PCM.UI.AddMoreSkillPoint();
        UpdateCastPoints();
    }

    private void CalculatePoints()
    {
        CurrentCastPoints = Mathf.FloorToInt(regenTimer / pointRegenRate);
    }

    public void InstantRegenPoint(float amount)
    {
        regenTimer = Mathf.Clamp(regenTimer + amount * pointRegenRate, 0, maxRegenTime);
        CalculatePoints();
    }

    public void InstantRegenPoint()
    {
        InstantRegenPoint(MaxCastPoints * 0.5f);
    }

    public void SpeedUpRegenDelay(float amount)
    {
        timer.ReduceCoolDown((int)SystemCD.pointRegenDelay, amount);
    }
    #endregion

    #region Health
    [Header("Health")]
    [SerializeField]
    private int startingHitPoint;
    private int actualMaxHealth;
    private void CalculateDamage(float damage)
    {
        if (!timer.IsTimeZero((int)SystemCD.iFrames) || PCM.control.CurrentState == playerState.consuming)
            return;
        if (Hitpoints - damage < 0)
        {
            OnDeath();
        }
        else
        {
            Hitpoints -= (int)Mathf.Ceil(damage);
            timer.SetTime((int)SystemCD.iFrames, iframes);
            PCM.control.SetHitStun(iframes);
        }
        SetHealthUI();
    }
    [ContextMenu("AttemptDamage")]
    public void AttemptDamage()
    {
        CalculateDamage(10);
    }

    public void FullHeal()
    {
        Hitpoints = actualMaxHealth;
        SetHealthUI();
    }

    public void Heal(int health)
    {
        Hitpoints += health;
        if (Hitpoints > actualMaxHealth) Hitpoints = actualMaxHealth;
        SetHealthUI();
    }

    public void HealByPercentage(int percentageToHeal)
    {
        if (percentageToHeal < 0) percentageToHeal = 0;

        int healAmount = (int)Mathf.Ceil(actualMaxHealth * percentageToHeal / 100);
        Heal(healAmount);
    }


    private void SetHealthUI()
    {
        float heathPercent = (float)Hitpoints / (float)actualMaxHealth;
        PCM.UI.SetGreenHealthBar(heathPercent);
    }

    public void UpgradeHealth()
    {
        actualMaxHealth = startingHitPoint + GameManager.Instance.StatsManager.bonusHealth;
        FullHeal();
    }

    #endregion

    #region Damage Interface
    [field: SerializeField]
    [field: ReadOnly]
    public int Hitpoints { get; set; }
    Rigidbody2D IDamageable.rb { get => PCM.control.rb; }

    public void OnDeath()
    {
        GameManager.Instance.EnemyManager.KillEnemies();
        GameManager.Instance.sceneLoader.LoadHub();

        GameManager.Instance.SetLostSouls();
        GameManager.Instance.SetSoulsToZero();
    }

    public void SetHitPoints()
    {
        actualMaxHealth = startingHitPoint + GameManager.Instance.StatsManager.bonusHealth;
        Hitpoints = actualMaxHealth;
    }

    public void TakeDamage(float amount, int staggerPoints, ElementType type, int tier, ElementType typeTwo = ElementType.noElement)
    {
        CalculateDamage(amount);
    }

    public void TakeDamage(float amount, int staggerPoints, ElementType type, ElementType typeTwo = ElementType.noElement)
    {
        CalculateDamage(amount);
    }

    public void AddForce(Vector2 force)
    {
        PCM.control.rb.velocity += force;
    }

    public void ModifySpeed(float percentage)
    {

    }

    public void ResetSpeed()
    {

    }
    #endregion

    #region Counter
    public bool isCountered { get; private set; }
    [Header("Counter Attack")]
    [SerializeField]
    private float counterQTEDuration;

    private Enemy target;
    private EnemyProjectile storedProjectile;

    [SerializeField]
    private int counterDamage;
    [SerializeField]
    private int counterStagger;
    [SerializeField]
    private LayerMask enemy;
    public void CounterSuccesful(Enemy target, EnemyProjectile projectile = null)
    {
        if (isCountered)
            return;
        isCountered = true;
        this.target = target;
        storedProjectile = projectile;
        timer.SetTime((int)SystemCD.counterAttackQTE, counterQTEDuration);
        PCM.control.CounteredAttack(counterQTEDuration);
    }

    [ContextMenu("test Counter")]
    private void TestCounter()
    {
        PCM.control.CounteredAttack(counterQTEDuration);
    }
    private void RemoveCounterQTE(object sender, EventArgs e)
    {
        RemoveCounterQTE();
    }

    private void RemoveCounterQTE()
    {
        isCountered = false;
        target = null;
        storedProjectile = null;
    }

    public void AttemptCounter(InputAction.CallbackContext context)
    {
        if (isCountered)
        {
            if (storedProjectile)
            {
                storedProjectile.CounterProjectile(target, transform.position, enemy, transform);
            }
            else
            {
                target.TakeDamage(counterDamage,counterStagger,ElementType.noElement);
            }
            RemoveCounterQTE();
        }
    }
    #endregion

    #region Consume
    [Header("Consume")]
    [SerializeField] private int consumeBar = 0;
    [SerializeField] private int consumeBarMax = 100;
    [SerializeField] private bool canConsume = false;

    public void AddToConsumeBar(int consumeValue)
    {
        consumeBar += consumeValue;
        PCM.UI.UpdateConsumeBar((float)consumeBar / (float)consumeBarMax);
        if (consumeBar > consumeBarMax)
        {
            canConsume = true;
        }
    }

    public void UseConsume(int percentageToHeal)
    {
        canConsume = false;
        consumeBar = 0;
        HealByPercentage(percentageToHeal);
        PCM.UI.EmptyConsumeBar();
    }

    public bool CanConsume()
    {
        return canConsume;
    }

    #endregion

    #region Getter
    public playerState GetState()
    {
        return PCM.control.CurrentState;
    }
    #endregion

    #region Debugging
    [ContextMenu("DebugKillPlayer")]
    public void KillPlayerDebug()
    {
        OnDeath();
    }
    #endregion
}
