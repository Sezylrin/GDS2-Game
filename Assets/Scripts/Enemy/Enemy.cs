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
    protected enum EnemyTimers
    {
        effectedTimer,
        staggerTimer,
        staggerScale
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
    [field: SerializeField] protected bool Staggered { get; set; } = false;
    [field: SerializeField] protected AudioSource WalkingSound { get; set; }
    [field: SerializeField] protected GameObject DeathSoundPrefab { get; set; }
    [field: SerializeField] protected Image HealthBarImage { get; set; }
    protected float HealthBarPercentage;
    [field: SerializeField] protected Image ElementEffectImage { get; set; }
    [field: SerializeField] protected GameObject StaggeredImage { get; set; }

    protected Timer Timer { get; private set; }

    [field: Header("Testing Variables")]
    [field: SerializeField] protected int EffectDuration { get; set; } = 5;
    [field: SerializeField] protected int StaggerDuration { get; set; } = 3;
    [field: SerializeField] protected int PointsToStagger { get; set; } = 100;

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
        Timer = TimerManager.Instance.GenerateTimers(typeof(EnemyTimers), gameObject);
        Timer.OnTimeIsZero += RemoveElementEffect;
        Timer.OnTimeIsZero += EndStagger;
    }

    protected virtual void Update()
    {

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
        /* if (CheckCombo() || CheckResistance())
        {
            CurrentHealth -= damage * damageMultiplier;
        }
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
        //if (Manager.CanAttack) Attack();
    }

    protected virtual void Attack()
    {
       //Attacking logic
    }

    protected virtual void ApplyElementEffect(ElementType type)
    {
        Timer.SetTime((int)EnemyTimers.effectedTimer, EffectDuration);
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

    protected virtual void RemoveElementEffect(object sender, Timer.OnTimeIsZeroEventArgs e)
    {
        if (e.timerSlot == (int)EnemyTimers.effectedTimer)
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
    }

    protected virtual void BeginStagger()
    {
        Timer.SetTime((int)EnemyTimers.staggerTimer, StaggerDuration);
        Staggered = true;
        StaggeredImage.SetActive(true);
    }

    protected virtual void EndStagger(object sender, Timer.OnTimeIsZeroEventArgs e)
    {
        if (e.timerSlot == (int)EnemyTimers.staggerTimer)
        {
            Staggered = false;
            StaggeredImage.SetActive(false);
        } 
    }

    protected virtual void AddToStaggerScale(int staggerPoints)
    {
        Timer.ReduceCoolDown((int)EnemyTimers.staggerScale, staggerPoints/-10);

        if (Timer.GetTime((int)EnemyTimers.staggerScale) * 10 >= PointsToStagger)
        {
            Timer.SetTime((int)EnemyTimers.staggerScale, 0);
            BeginStagger();
        }
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
