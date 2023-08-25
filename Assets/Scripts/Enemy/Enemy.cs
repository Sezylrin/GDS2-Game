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

public enum Element
{
    NoElement,
    Fire,
    Water,
    Shock,
    Poison,
    Wind,
    Nature
}

public abstract class Enemy : MonoBehaviour
{
    [field: Header("Enemy Info")]
    [field: SerializeField] public EnemyType Type { get; set; }
    [field: SerializeField] public Element Element { get; set; }
    [field: SerializeField] public float CurrentHealth { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; } 
    [field: SerializeField] public float Damage { get; set; }
    [field: SerializeField] public float Speed { get; set; }
    [field: SerializeField, ReadOnly] public float Souls { get; set; }

    [field: Header("Other")]
    
    [field: SerializeField] public AudioSource WalkingSound { get; set; }
    [field: SerializeField] public AudioSource DeathSound { get; set; }

    //public EnemyManager Manager { get; set; }
    //punlic Player Player { get; set; }

    protected virtual void Init()
    {
        //Manager = GameManager.EnemyManager;
        SetStats();
    }

    protected virtual void Awake()
    {
        Init();
    }

    public virtual void SetStats()
    {
        CurrentHealth = MaxHealth;
    }

    protected virtual void TakeDamage(float damage, Element damageType)
    {
        /* if (CheckCombo() || CheckResistance())
        {
            CurrentHealth -= damage * damageMultiplier;
        }
        else */ CurrentHealth -= damage;
        if (CurrentHealth <= 0) OnDeath();
    }

    protected virtual void OnDeath()
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
}
