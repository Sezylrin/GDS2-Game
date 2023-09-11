using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private int soulCost;
    [SerializeField]
    private string skillName;
    [SerializeField]
    private string skillDescription;

    private bool canAfford = false;
    private bool hovering = false;
    private bool purchased = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        skillTreeManager = GameObject.FindGameObjectWithTag("SkillTreeCanvas").GetComponent<SkillTreeManager>();
        canAfford = gameManager.Souls >= soulCost;
        Image bgImage = gameObject.GetComponent<Image>();

        if (!canAfford && !purchased)
        {
            bgImage.sprite = bgSpriteDisabled;
        }
    }

    private void Update()
    {
        canAfford = gameManager.Souls >= soulCost;
        Image bgImage = gameObject.GetComponent<Image>();

        if (!canAfford && !hovering && !purchased)
        {
            bgImage.sprite = bgSpriteDisabled;
        }
        else if (canAfford && !hovering)
        {
            bgImage.sprite = bgSprite;
        }
    }

    public void ActivateHover()
    {
        hovering = true;
        Image bgImage = gameObject.GetComponent<Image>();
        if (!canAfford && !purchased)
        {
            bgImage.sprite = bgSpriteHoveredDisabled;
        }
        else
        {
            bgImage.sprite = bgSpriteHovered;
        }

        skillTreeManager.ShowSkillTree(skillName, skillDescription, soulCost, purchased);
    }

    public void DisableHover()
    {
        Image bgImage = gameObject.GetComponent<Image>();
        if (!canAfford && !purchased)
        {
            bgImage.sprite = bgSpriteDisabled;
        }
        else
        {
            bgImage.sprite = bgSprite;
        }
        hovering = false;

        skillTreeManager.HideSkillTree();
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
        if (canAfford)
        {
            gameManager.Souls -= soulCost;
            purchased = true;
            skillTreeManager.ShowPurchased();
        } 
    }
}
