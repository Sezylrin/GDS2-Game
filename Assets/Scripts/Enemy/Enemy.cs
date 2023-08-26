using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Type1,
    Type2, 
    Type3, 
    Type4, 
    Type5, 
    etc
}


public abstract class Enemy : MonoBehaviour, IDamageable
{
    [field: Header("Enemy Info")]
    [field: SerializeField] public EnemyType Type { get; set; }
    [field: SerializeField] public ElementType Element { get; set; } = ElementType.noElement;
    [field: SerializeField] public float Hitpoints { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; } 
    [field: SerializeField] public float Damage { get; set; }
    [field: SerializeField] public float Speed { get; set; }
    [field: SerializeField, ReadOnly] public float Souls { get; set; }
    [field: SerializeField] public ElementType ActiveElementEffect { get; set; }

    [field: Header("Other")]
    [field: SerializeField] public AudioSource WalkingSound { get; set; }
    [field: SerializeField] public AudioSource DeathSound { get; set; }

    [field: Header("Testing Variables")]
    [field: SerializeField] public int EffectDuration { get; set; } = 5;

    protected IEnumerator effectTimerCoroutine;

   
    //public EnemyManager Manager { get; set; }
    //punlic Player Player { get; set; }

    protected virtual void Init()
    {
        //Manager = GameManager.EnemyManager;
        SetStats();
        ActiveElementEffect = Element;
        effectTimerCoroutine = EffectTimer();
    }

    protected virtual void Awake()
    {
        Init();
    }

    public virtual void SetStats()
    {
        Hitpoints = MaxHealth;
    }

    public void SetHitPoints()
    {

    }

    public virtual void TakeDamage(float damage, ElementType type)
    {
        /* if (CheckCombo() || CheckResistance())
        {
            CurrentHealth -= damage * damageMultiplier;
        }
        else */ Hitpoints -= damage;
        if (Hitpoints <= 0) OnDeath();
        ApplyElementEffect(type);
    }

    public virtual void OnDeath()
    {
        if (DeathSound) DeathSound.Play();
        Destroy(gameObject); //Temp -> Replace with pooling
    }

    protected virtual void AttemptAttack() //Check if Enemy Manager has an attack point available
    {
        //if (Manager.CanAttack) Attack()
    }

    protected virtual void Attack()
    {
       //Attacking logic
    }

    protected virtual void ApplyElementEffect(ElementType type)
    {
        StopCoroutine("EffectTimer");
        ActiveElementEffect = type;
        StartCoroutine("EffectTimer", 0f);
    }

    protected virtual void RemoveElementEffect()
    {
        ActiveElementEffect = Element;
    }

    protected virtual IEnumerator EffectTimer()
    {
        yield return new WaitForSeconds(EffectDuration);
        RemoveElementEffect();
  
    }
}
