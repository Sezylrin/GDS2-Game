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
        if (CanPurchase())
        {
            BackgroundColor.color = GetBackgroundColor();
        }
        else
        {
            BackgroundColor.color = CantAffordColor;
        }
        SoulCost = SoulCostToSet;
        Icon.sprite = abilityData.icon;
    }

    public override void ActivateHover()
    {
        base.ActivateHover();
        GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdatePopup(abilityData.name, abilityData.description, CanPurchase(), purchased);
    }

    public override void HandlePurchase()
    {
        if (!CanPurchase())
        {
            GameManager.Instance.AudioManager.PlaySound(AudioRef.ButtonPressFail);
            return;
        }

        purchased = true;
        GameManager.Instance.StatsManager.UnlockAbility(abilityData);
        GameManager.Instance.RemoveSouls(SoulCost);
        GameManager.Instance.AudioManager.PlaySound(AudioRef.buttonPress);
        GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdatePopup(abilityData.name, abilityData.description, CanPurchase(), purchased);
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
