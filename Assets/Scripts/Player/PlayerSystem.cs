using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class PlayerSystem : MonoBehaviour, IDamageable
{
    private void Start()
    {
        SetHitPoints();
    }

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

    public void TakeDamage(float amount)
    {
        CalculateDamage(amount);
    }

    
    #endregion
}
