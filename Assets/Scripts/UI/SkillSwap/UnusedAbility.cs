using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnusedAbility : UIAbility
{
    public override void ActivateHover(bool playSound = true)
    {
        GameManager.Instance.UIManager.GetBookMenu().SkillSwitch.GetComponent<SkillSwitch>().UpdatePopup(abilityData);
        if (playSound)
        {
            GameManager.Instance.AudioManager.PlaySound(AudioRef.ButtonHover);
        }
        base.PlayHoverAnimation();
    }

    public override void DisableHover()
    {
        Image bgImage = gameObject.GetComponent<Image>();

        if (GameManager.Instance.StatsManager.GetUnlockedAbilities().Contains(abilityData))
        {
            borderImage.sprite = bg;
        }
        else
        {
            borderImage.sprite = disabledBg;
        }

        base.StopHoverAnimation();
    }

    public void UpdateBorder()
    {
        borderImage.sprite = bg;
    }
}
