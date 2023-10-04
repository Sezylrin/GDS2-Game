using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

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

    private Tween hoverTween;
    protected AudioComponent audioComponent;

    private void Start()
    {
        gameManager = GameManager.Instance;
        skillTreeManager = GameObject.FindGameObjectWithTag("SkillTreeCanvas").GetComponent<SkillTreeManager>();
        canAfford = gameManager.Souls >= soulCost;
        Image bgImage = gameObject.GetComponent<Image>();
        audioComponent = gameObject.GetComponent<AudioComponent>();

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
        PlayHoverAnimation();
        audioComponent.PlaySound(SoundType.UIHover);
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
        StopHoverAnimation();
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
        if (!canAfford || purchased)
        {
            audioComponent.PlaySound(SoundType.UIClickFail);
            return;
        }
        foreach (SkillTreeButton skillTreeBtn in prerequisiteSkills)
        {
            if (!skillTreeBtn.purchased)
            {
                audioComponent.PlaySound(SoundType.UIClickFail);
                return;
            };
        }
        audioComponent.PlaySound(SoundType.UIUnlockSkill);
        gameManager.RemoveSouls(soulCost);
        purchased = true;
        skillTreeManager.ShowPurchased();
        skillTreeManager.UpdateSoulsText();
        onPurchase.Invoke();
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

    protected void StopHoverAnimation()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        hoverTween?.Kill();
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected void PlayHoverAnimation()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        hoverTween?.Kill();
        hoverTween = DOTween.Sequence()
            .Append(rectTransform.DOScale(new Vector3(1.05f, 1.05f, 1f), 0.3f).SetEase(Ease.InOutSine))
            .Append(rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo)
            .Play();
    }
}
