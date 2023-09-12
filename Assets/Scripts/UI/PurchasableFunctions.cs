using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableFunctions : MonoBehaviour
{
    //Element Damage by large amount
    //Player Health
    //Increase skill point total
    //Player damage by tiny amount
    public void IncreaseHealth()
    {
        Debug.Log("Increased Health");
    }

    public void IncreaseStatus()
    {
        Debug.Log("Increase Status");
    }
}

/*
 * Give GameManager access to Player and PlayerComponentManager
 * Health Stuff: On Player Systems is Health, Calculate Damage, add a function that modifies the hitpoints. Add max health will heal them for that amount. Modify starting Hitpoints. Might want to add a maxHitPoint float
 * Damage Stuff: 
 * Abilities: Singleton on a GameManager that contains an int variable that tells you bonuys damage that starts at 0. A list or dictionary of floats, each one is bonusDamage for elements. Every time you increase the skillpoint, up the bonusDamage by 1.  AbilityBase onTriggferEnter2d after hitEnemy.add, do int var = selectedAbility.damage + modifierDamage + elementDamage based on element type
 * Selected ability has type and damage that i can use to add onto the modifier
 * 
 * Skill points: PlayerSystems, set MaxCastPoints (in Region Ability), should increase by however much the skill points add. make InitCastPoints public. once applying skill, call InitCastPoints again.
 */