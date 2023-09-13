using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameManager gameManager;
    private SkillTreeManager skillTreeManager;

    [Header("Icons")]
    [SerializeField]
    private Sprite bgSprite;
    [SerializeField]
    private Sprite bgSpriteHovered;
    [SerializeField]
    private Sprite bgSpriteDisabled;
    [SerializeField]
    private Sprite bgSpriteHoveredDisabled;

    [Header("Data")]
    [SerializeField]
    private UnityEvent onPurchase;
    [SerializeField]
    private int soulCost;
    [SerializeField]
    private string skillName;
    [SerializeField]
    private string skillDescription;
    [SerializeField]
    private SkillTreeButton[] prerequisiteSkills;

    private bool canAfford = false;
    private bool hovering = false;
    public bool purchased { get; private set; } = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        skillTreeManager = GameObject.FindGameObjectWithTag("SkillTreeCanvas").GetComponent<SkillTreeManager>();
        canAfford = gameManager.Souls >= soulCost;
        Image bgImage = gameObject.GetComponent<Image>();

        if ((!canAfford && !purchased) || !PrereqUnlocked())
        {
            bgImage.sprite = bgSpriteDisabled;
        }
    }

    private void Update()
    {
        canAfford = gameManager.Souls >= soulCost;
        Image bgImage = gameObject.GetComponent<Image>();

        if ((!canAfford && !hovering && !purchased))
        {
            bgImage.sprite = bgSpriteDisabled;
        }
        else if (canAfford && !hovering && PrereqUnlocked())
        {
            bgImage.sprite = bgSprite;
        }
    }

    public void ActivateHover()
    {
        hovering = true;
        Image bgImage = gameObject.GetComponent<Image>();
        if ((!canAfford && !purchased) || !PrereqUnlocked())
        {
            bgImage.sprite = bgSpriteHoveredDisabled;
        }
        else
        {
            bgImage.sprite = bgSpriteHovered;
        }
        
        skillTreeManager.ShowSkillTreePopup(skillName, skillDescription, soulCost, purchased, PrereqUnlocked());
    }

    public void DisableHover()
    {
        Image bgImage = gameObject.GetComponent<Image>();
        if ((!canAfford && !purchased) || !PrereqUnlocked())
        {
            bgImage.sprite = bgSpriteDisabled;
        }
        else
        {
            bgImage.sprite = bgSprite;
        }
        hovering = false;

        skillTreeManager.HideSkillTreePopup();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActivateHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DisableHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (SkillTreeButton skillTreeBtn in prerequisiteSkills)
        {
            if (!skillTreeBtn.purchased) return;
        }
        if (canAfford)
        {
            gameManager.Souls -= soulCost;
            purchased = true;
            skillTreeManager.ShowPurchased();
            skillTreeManager.UpdateSoulsText();
            onPurchase.Invoke();
        } 
    }

    private bool PrereqUnlocked()
    {
        bool preReqUnlocked = true;

        foreach (SkillTreeButton skillTreeBtn in prerequisiteSkills)
        {
            if (!skillTreeBtn.purchased) preReqUnlocked = false;
        }

        return preReqUnlocked;
    }
}
