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
    }

    public void IncreaseElementalDamage(int AmountToIncrease)
    {
        GameManager.Instance.StatsManager.abilityModifier += AmountToIncrease;
    }

    public void IncreaseDamage(int AmountToIncrease)
    {
        GameManager.Instance.StatsManager.attackDamageModifier += AmountToIncrease;
    }

    public void UnlockSecondSkillSet()
    {
        GameManager.Instance.StatsManager.secondSkillsetUnlocked = true;
    }

    public void SkillUnlock(ElementalSO skill)
    {
        GameManager.Instance.StatsManager.UnlockAbility(skill);
    }
}
