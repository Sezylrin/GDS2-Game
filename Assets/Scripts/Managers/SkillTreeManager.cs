using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField]
    private GameObject skillTreePopup;
    [SerializeField]
    private GameObject cantAffordText;
    [SerializeField]
    private GameObject purchasedText;
    [SerializeField]
    private TMP_Text soulCostTxt;
    [SerializeField]
    private TMP_Text skillNameTxt;
    [SerializeField]
    private TMP_Text skillDescriptionTxt;
    [SerializeField] 
    private TMP_Text soulsTxt;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideSkillTree();
        }
    }

    public void ShowSkillTree()
    {
        gameObject.SetActive(true);
        GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
        UpdateSoulsText();
    }

    public void HideSkillTree()
    {
        GameManager.Instance.UIManager.PlayCloseMenuSound();
        gameObject.SetActive(false);
    }

    public void ShowSkillTreePopup(string skillName, string skillDescription, int skillSoulCost, bool purchased, bool prereqUnlocked)
    {
        skillNameTxt.text = skillName;
        skillDescriptionTxt.text = skillDescription;
        soulCostTxt.text = skillSoulCost.ToString() + " Souls";

        if ((GameManager.Instance.Souls < skillSoulCost && !purchased) || !prereqUnlocked)
        {
            if (!prereqUnlocked) cantAffordText.GetComponent<TMP_Text>().text = "Previous skill not unlocked";
            cantAffordText.SetActive(true);
        }
        else if (purchased)
        {
            purchasedText.SetActive(true);
        }
        skillTreePopup.SetActive(true);
    }

    public void HideSkillTreePopup()
    {
        cantAffordText.GetComponent<TMP_Text>().text = "Can't Afford";
        cantAffordText.SetActive(false);
        skillTreePopup.SetActive(false);
        purchasedText.SetActive(false);
    }

    public void ShowPurchased()
    {
        purchasedText.SetActive(true);
    }

    public void UpdateSoulsText()
    {
        soulsTxt.text = GameManager.Instance.Souls.ToString();
    }
}
