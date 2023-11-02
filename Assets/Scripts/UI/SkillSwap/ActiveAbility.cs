using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAbility : UIAbility
{
    public int abilityIndex;

    public void UpdateAbility(ElementalSO newAbilityData)
    {
        abilityData = newAbilityData;
        abilityIcon.sprite = newAbilityData.icon;
        backgroundColor.color = GetBackgroundColor();
        backgroundColor.enabled = true;
        abilityIcon.enabled = true;
        borderImage.sprite = bg;
    }

    public void GreyBorder()
    {
        borderImage.sprite = disabledBg;
    }
}
