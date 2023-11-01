using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    UnlockSecondSkillset,
    UpgradeHealth,
    UpgradeDamage,
}

public class UpgradeSkillTreeButton : BaseSkillTreeButton
{
    [SerializeField] private int SoulCostToSet;
    [SerializeField] private Sprite IconToSet;
    [SerializeField] private string Name;
    [SerializeField] private string Description;
    [SerializeField] bool CanPurchaseOnce;
    [SerializeField] UpgradeType UpgradeType;

    int TimesPurchased = 1;

    public override void Init()
    {
        SoulCost = SoulCostToSet;
        if (CanPurchase())
        {
            BackgroundColor.color = new Color(BackgroundColor.color.r, BackgroundColor.color.g, BackgroundColor.color.b, 0f);
        }
        else
        {
            BackgroundColor.color = CantAffordColor;
        }
        Icon.sprite = IconToSet;
    }
    public override void ActivateHover()
    {
        base.ActivateHover();
        GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdatePopup(Name, Description, CanPurchase(), purchased);
    }

    public override void HandlePurchase()
    {
        if (CanPurchase())
        {
            if (purchased && CanPurchaseOnce)
            {
                GameManager.Instance.AudioManager.PlaySound(AudioRef.ButtonPressFail);
                return;
            }
            if (CanPurchaseOnce)
            {
                BackgroundColor.color = new Color(BackgroundColor.color.r, BackgroundColor.color.g, BackgroundColor.color.b, 0f);
                purchased = true;
            }
            else
            {
                TimesPurchased++;
                Name += " " + TimesPurchased;
            }
            GameManager.Instance.RemoveSouls(SoulCost);
            GameManager.Instance.AudioManager.PlaySound(AudioRef.buttonPress);
            GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdatePopup(Name, Description, CanPurchase(), purchased);
            GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdateSoulsText(GameManager.Instance.Souls);

            switch (UpgradeType)
            {
                case UpgradeType.UnlockSecondSkillset:
                    UnlockSecondSkillset();
                    break;
                case UpgradeType.UpgradeHealth:
                    UpgradeHealth();
                    break;
                case UpgradeType.UpgradeDamage:
                    UpgradeDamage();
                    break;
            }
        }
        else
        {
            GameManager.Instance.AudioManager.PlaySound(AudioRef.ButtonPressFail);
        }
    }

    public void UnlockSecondSkillset()
    {
        GameManager.Instance.StatsManager.secondSkillsetUnlocked = true;
    }

    public void UpgradeHealth()
    {
        GameManager.Instance.StatsManager.bonusHealth += 10;
        GameManager.Instance.PCM.system.UpgradeHealth();
    }

    public void UpgradeDamage()
    {
        GameManager.Instance.StatsManager.abilityModifier += 10;
        GameManager.Instance.StatsManager.attackDamageModifier += 10;
    }
}
