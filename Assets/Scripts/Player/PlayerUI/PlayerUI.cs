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
    private GameObject abilities;
    [SerializeField]
    private TMP_Text QAbilityText;
    [SerializeField]
    private TMP_Text EAbilityText;
    [SerializeField]
    private TMP_Text shiftAbilityText;
    [Header("Souls UI")]
    [SerializeField]
    private TMP_Text soulsTxt;
    [Header("Consume UI")]
    [SerializeField]
    private Image consumeBarImage;

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

    public void UpdateSoulsText()
    {
        soulsTxt.text = GameManager.Instance.Souls.ToString();
    }

    public void UpdateConsumeBar(float fillAmount)
    {
        consumeBarImage.fillAmount = fillAmount;
        if (PCM.system.CanConsume())
        {
            consumeBarImage.color = Color.red;
        }
    }

    public void EmptyConsumeBar()
    {
        consumeBarImage.fillAmount = 0;
        consumeBarImage.color = Color.yellow;
    }

    public void EnableAbilityUI()
    {
        abilities.SetActive(true);
    }

    public void DisableAbilityUI()
    {
        if (GameManager.Instance.IsTutorial)
            abilities.SetActive(false);
    }
}
