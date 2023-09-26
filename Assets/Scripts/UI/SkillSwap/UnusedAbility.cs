using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnusedAbility : MonoBehaviour
{
    [SerializeField]
    private Sprite bg;
    [SerializeField]
    private Sprite hoverBg;
    [SerializeField]
    private Sprite disabledBg;
    [SerializeField]
    private Sprite hoveredDisabledBg;

    [HideInInspector]
    public ElementalSO abilityData;
    [HideInInspector]
    public SkillSwitchManager skillSwitchManager;

    public void Start()
    {
        StatsManager statsManager = GameManager.Instance.StatsManager;

        if (!statsManager.GetUnlockedAbilities().Contains(abilityData))
        {
            gameObject.GetComponent<Image>().sprite = disabledBg;
        }
    }

    public void ActivateHover()
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
    }

    public void DisableHover()
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
    }
}
