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
    [Header("Active SKills")]
    [SerializeField]
    private GameObject abilities;
    [SerializeField]
    private TMP_Text abilityText1;
    [SerializeField]
    private TMP_Text abilityText2;
    [SerializeField]
    private TMP_Text abilityText3;
    [SerializeField]
    private TMP_Text abilityText4;
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

    
    public void UpdateAbilityText(string AbilityOne, string AbilityTwo, string AbilityThree, string AbilityFour)
    {
        abilityText1.text = AbilityOne;
        abilityText2.text = AbilityTwo;
        abilityText3.text = AbilityThree;
        abilityText4.text = AbilityFour;
    }

    public void UpdateSoulsText()
    {
        soulsTxt.text = GameManager.Instance.Souls.ToString();
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
