using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
public class PlayerSystem : MonoBehaviour, IDamageable
{
    private enum SystemCD
    {
        pointRegenDelay
    }

    [Header("General")]
    [SerializeField]
    private Timer timer;
    [SerializeField]
    private PlayerComponentManager PCM;

    private void Start()
    {
        SetHitPoints();
        timer = TimerManager.Instance.GenerateTimers(typeof(SystemCD), gameObject);
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
    private int CurrentCastPoints;
    [SerializeField]
    private float pointRegenRate;
    [SerializeField][ReadOnly]
    private float regenTimer;
    [SerializeField]
    private float regenDelay;
    private float maxRegenTime;
    [SerializeField]
    private int debugCost;

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
        maxRegenTime = MaxCastPoints * pointRegenRate;
        CurrentCastPoints = MaxCastPoints;
        regenTimer = maxRegenTime;
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
    public void CalculateDamage(float amount)
    {
        if (Hitpoints - amount < 0)
        {
            OnDeath();
        }
        else
        {
            Hitpoints -= amount;
        }
    }
    #endregion

    #region Damage Interface
    [field: SerializeField][field:ReadOnly]
    public float Hitpoints { get; set; }
    public void OnDeath()
    {
    }

    public void SetHitPoints()
    {
        Hitpoints = startingHitPoint;
    }

    public void TakeDamage(float amount, ElementType type)
    {
        CalculateDamage(amount);
    }

    
    #endregion
}
