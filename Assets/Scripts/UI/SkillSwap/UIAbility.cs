using UnityEngine;
using UnityEngine.UI;

public class UIAbility : MonoBehaviour
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
        GameManager.Instance.SkillSwitchManager.UpdatePopup(abilityData);
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
