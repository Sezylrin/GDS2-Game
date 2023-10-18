using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnusedAbility : UIAbility
{
    public override void ActivateHover(bool playSound = true)
    {
        Image bgImage = gameObject.GetComponent<Image>();

        if (GameManager.Instance.StatsManager.GetUnlockedAbilities().Contains(abilityData))
        {
            bgImage.sprite = hoverBg;
        }
        else
        {
            bgImage.sprite = hoveredDisabledBg;
        }
        GameManager.Instance.BookMenu.SkillSwitch.GetComponent<SkillSwitch>().UpdatePopup(abilityData);
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
            bgImage.sprite = bg;
        }
        else
        {
            bgImage.sprite = disabledBg;
        }

        base.StopHoverAnimation();
    }

    public void UpdateBorder()
    {
        Image bgImage = gameObject.GetComponent<Image>();
        bgImage.sprite = bg;
    }
}
