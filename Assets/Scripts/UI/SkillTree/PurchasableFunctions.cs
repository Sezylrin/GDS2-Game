using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableFunctions : MonoBehaviour
{
    public void IncreaseHealth(int AmountToIncrease)
    {
        GameManager.Instance.StatsManager.bonusHealth += AmountToIncrease;
        GameManager.Instance.PCM.system.UpgradeHealth();
    }

    public void IncreaseMaxSkillPoints(int AmountToIncrease)
    {
        GameManager.Instance.PCM.system.AddCastPoint();
    }

    public void IncreaseElementalDamage(int AmountToIncrease)
    {
        GameManager.Instance.StatsManager.abilityModifier += AmountToIncrease;
    }

    public void IncreaseDamage(int AmountToIncrease)
    {
        GameManager.Instance.StatsManager.attackDamageModifier += AmountToIncrease;
    }
}