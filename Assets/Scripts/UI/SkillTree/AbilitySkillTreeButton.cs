using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AbilitySkillTreeButton : BaseSkillTreeButton
{
    [SerializeField] protected ElementalSO abilityData;
    [SerializeField] private int SoulCostToSet;

    public override void Init()
    {
        SoulCost = SoulCostToSet;
        if (CanPurchase())
        {
            BackgroundColor.color = GetBackgroundColor();
        }
        else
        {
            BackgroundColor.color = CantAffordColor;
        }
        Icon.sprite = abilityData.icon;
    }

    public override void ActivateHover()
    {
        base.ActivateHover();
        UpdatePopup();
    }

    public override void UpdatePopup()
    {
        string Description = abilityData.description + ". Costs " + SoulCost + " souls to purchase.";
        GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdatePopup(abilityData.name, Description, CanPurchase(), purchased);
    }

    public override void HandlePurchase()
    {
        if (!CanPurchase() || purchased)
        {
            GameManager.Instance.AudioManager.PlaySound(AudioRef.ButtonPressFail);
            return;
        }

        purchased = true;
        GameManager.Instance.StatsManager.UnlockAbility(abilityData);
        GameManager.Instance.RemoveSouls(SoulCost);
        GameManager.Instance.AudioManager.PlaySound(AudioRef.buttonPress);
        UpdatePopup();
        GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdateSoulsText(GameManager.Instance.Souls);
    }

    public Color GetBackgroundColor()
    {
        switch (abilityData.elementType)
        {
            case ElementType.fire:
                Color fireColor = new Color(1f, 0.3f, 0.3f, 1f);
                return fireColor;
            case ElementType.water:
                Color waterColor = new Color(0.3f, 0.5f, 1f, 1f);
                return waterColor;
            case ElementType.wind:
                Color windColor = new Color(0.53f, 0.81f, 0.98f, 1f);
                return windColor;
            case ElementType.electric:
                Color electricColor = new Color(1f, 1f, 0.5f, 1f);
                return electricColor;
            default:
                return Color.white;
        }
    }
}
