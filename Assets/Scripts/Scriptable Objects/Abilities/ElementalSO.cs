using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    blast,
    AOE,
    Projectile,
    dash
}
public class ElementalSO : ScriptableObject
{
    public Material color;

    public ElementType elementType;

    public AbilityType type;

    public int castCost;

    public float castStartSpeed;

    public float castDuration;

    public int pierceAmount;

    public int damage;

    public float knockback;

    public int Stagger;
    public int consumePoints;

    public float lifeTime;

    public string description;

    public Sprite icon;


}





