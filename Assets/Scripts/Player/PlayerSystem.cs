using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using System;
public class PlayerSystem : MonoBehaviour, IDamageable
{
    private enum SystemCD
    {
        pointRegenDelay,
        iFrames
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
        timer = GameManager.Instance.TimerManager.GenerateTimers(typeof(SystemCD), gameObject);
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
    [SerializeField][ReadOnly]
    private int currentCastPoints;
    [SerializeField]
    private float pointRegenRate;
    [SerializeField][ReadOnly]
    private float regenTimer;
    [SerializeField]
    private float regenDelay;
    private float maxRegenTime;
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
        maxRegenTime = MaxCastPoints * pointRegenRate;
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
        regenTimer += amount * pointRegenRate;
        CalculatePoints();
    }
    #endregion

    #region Health
    [Header("Health")]
    [SerializeField]
    private float startingHitPoint;
    private void CalculateDamage(float amount)
    {
        if (!timer.IsTimeZero((int)SystemCD.iFrames))
            return;
        if (Hitpoints - amount < 0)
        {
            OnDeath();
        }
        else
        {
            Hitpoints -= amount;
            timer.SetTime((int)SystemCD.iFrames, iframes);
        }
        SetHealthUI();
    }

    public void FullHeal()
    {
        Hitpoints = startingHitPoint + GameManager.Instance.StatsManager.bonusHealth;
        SetHealthUI();
    }

    private void SetHealthUI()
    {
        PCM.UI.SetGreenHealthBar(Hitpoints / (startingHitPoint + GameManager.Instance.StatsManager.bonusHealth));
    }

    public void UpgradeHealth()
    {
        FullHeal();
    }
    #endregion

    #region Damage Interface
    [field: SerializeField][field:ReadOnly]
    public float Hitpoints { get; set; }
    Rigidbody2D IDamageable.rb { get => PCM.control.rb; }

    public void OnDeath()
    {
        GameManager.Instance.EnemyManager.KillEnemies();
        GameManager.Instance.SetSoulsToZero();
        Loader.Load(EN_Scene.Sprint2);
    }

    public void SetHitPoints()
    {
        Hitpoints = startingHitPoint;
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

    }

    public void ModifySpeed(float percentage)
    {
        
    }

    public void ResetSpeed()
    {
        
    }
    #endregion
}
