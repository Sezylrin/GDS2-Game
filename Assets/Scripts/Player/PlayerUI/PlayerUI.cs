using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private PlayerComponentManager PCM;

    [Header("Health UI")]
    [SerializeField]
    private Image greenHealth;
    [Header("SkillPoints")]
    [SerializeField]
    private Transform skillPointHoriLayout;
    [SerializeField]
    private GameObject skillPointPrefab;
    private List<SkillPointUI> skillPoints = new List<SkillPointUI>();
    [Header("Active SKills")]
    [SerializeField]
    private TMP_Text QAbilityText;
    [SerializeField]
    private TMP_Text EAbilityText;
    [SerializeField]
    private TMP_Text shiftAbilityText;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGreenHealthBar(float percentage)
    {
        greenHealth.fillAmount = percentage;
    }

    public void AddMoreSkillPoint()
    {
        skillPoints.Add(Instantiate(skillPointPrefab, skillPointHoriLayout).GetComponent<SkillPointUI>());
    }

    public void UpdateSKillPointUI(int availableSkillPoint)
    {
        for (int i = 0; i < skillPoints.Count; i++)
        {
            if (i < availableSkillPoint)
            {
                skillPoints[i].EnableSkillPoint();
            }
            else
            {
                skillPoints[i].DisableSkillPoint();
            }
        }
    }

    public void UpdateAbilityText(string AbilityOne, string AbilityTwo, string AbilityThree)
    {
        QAbilityText.text = AbilityOne;
        EAbilityText.text = AbilityTwo;
        shiftAbilityText.text = AbilityThree;
    }
}
