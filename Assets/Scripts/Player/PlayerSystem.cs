using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
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
    }
    #endregion

    #region Damage Interface
    [field: SerializeField][field:ReadOnly]
    public float Hitpoints { get; set; }
    Rigidbody2D IDamageable.rb { get => PCM.control.rb; }

    public void OnDeath()
    {
        Debug.Log("oh, i'm die, thank you forever");
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
    #endregion
}
