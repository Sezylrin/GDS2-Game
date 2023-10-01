using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIAbility : MonoBehaviour
{
    [SerializeField]
    protected Sprite bg;
    [SerializeField]
    protected Sprite hoverBg;
    [SerializeField]
    protected Sprite disabledBg;
    [SerializeField]
    protected Sprite hoveredDisabledBg;

    [HideInInspector]
    public ElementalSO abilityData;

    private Tween hoverTween;
    protected AudioComponent audioComponent;

    private void Awake()
    {
        audioComponent = GetComponent<AudioComponent>();
    }

    public void Start()
    {
        StatsManager statsManager = GameManager.Instance.StatsManager;

        if (!statsManager.GetUnlockedAbilities().Contains(abilityData))
        {
            gameObject.GetComponent<Image>().sprite = disabledBg;
        }
    }

    public virtual void ActivateHover()
    {
        Image bgImage = gameObject.GetComponent<Image>();

        if (abilityData != null)
        {
            bgImage.sprite = hoverBg;
        }
        else
        {
            bgImage.sprite = hoveredDisabledBg;
        }
        GameManager.Instance.SkillSwitchManager.UpdatePopup(abilityData);
        audioComponent.PlaySound(SoundType.UIHover);
        PlayHoverAnimation();
    }

    public virtual void DisableHover()
    {
        Image bgImage = gameObject.GetComponent<Image>();

        if (abilityData != null)
        {
            bgImage.sprite = bg;
        }
        else
        {
            bgImage.sprite = disabledBg;
        }

        StopHoverAnimation();
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
