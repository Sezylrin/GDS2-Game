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
    [SerializeField] private string OriginalName;
    [SerializeField] private string Description;
    [SerializeField] bool CanPurchaseOnce;
    [SerializeField] UpgradeType UpgradeType;

    int TimesPurchased = 1;
    private string UpdatedName;

    public override void Init()
    {
        SoulCost = SoulCostToSet;
        if (!CanPurchaseOnce)
        {
            UpdatedName = OriginalName + " " + TimesPurchased;
        }
        else
        {
            UpdatedName = OriginalName;
        }
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

    public override void UpdatePopup()
    {
        string UpdatedDescription = Description + ". Costs " + SoulCost + " souls to purchase.";
        GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdatePopup(UpdatedName, UpdatedDescription, CanPurchase(), purchased);
    }

    public override void ActivateHover()
    {
        base.ActivateHover();
        UpdatePopup();
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
                UpdatedName = OriginalName + " " + TimesPurchased;
                SoulCost += 50;
            }
            GameManager.Instance.RemoveSouls(SoulCost);
            GameManager.Instance.AudioManager.PlaySound(AudioRef.buttonPress);
            UpdatePopup();
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
        GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdateHealthText();
    }

    public void UpgradeDamage()
    {
        GameManager.Instance.StatsManager.damageModifier += 0.1f;
        GameManager.Instance.UIManager.GetBookMenu().SkillTree.GetComponent<BookSkillTree>().UpdateDamageText();
    }
}
