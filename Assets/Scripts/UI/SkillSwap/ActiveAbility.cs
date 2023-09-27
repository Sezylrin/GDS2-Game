using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAbility : UIAbility
{
    public void UpdateAbility(ElementalSO newAbilityData)
    {
        abilityData = newAbilityData;
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = newAbilityData.icon;
    }
}
