using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAbility : UIAbility
{
    public void UpdateAbility(ElementalSO newAbilityData)
    {
        abilityData = newAbilityData;
        Image image = gameObject.transform.GetChild(0).GetComponent<Image>();
        image.sprite = newAbilityData.icon;
        image.enabled = true;
        gameObject.GetComponent<Image>().sprite = bg;
    }

    public void GreyBorder(bool hovered = false)
    {
        if (hovered)
        {
            gameObject.GetComponent<Image>().sprite = hoveredDisabledBg;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = disabledBg;
        }
    }
}
