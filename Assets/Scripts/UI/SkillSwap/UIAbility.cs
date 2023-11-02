using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIAbility : MonoBehaviour
{
    [SerializeField] protected Sprite bg;
    [SerializeField] protected Sprite disabledBg;
    [SerializeField] protected Image borderImage;
    [SerializeField] protected Image backgroundColor;
    [SerializeField] protected Image abilityIcon;

    [HideInInspector]
    public ElementalSO abilityData;

    private Tween hoverTween;
    protected Color CantUseColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    public void Start()
    {
        StatsManager statsManager = GameManager.Instance.StatsManager;

        if (!statsManager.GetUnlockedAbilities().Contains(abilityData))
        {
            borderImage.sprite = disabledBg;
        }
    }

    public void InitIconAndBackground(bool canUse)
    {
        if (!abilityData) return;
        backgroundColor.enabled = true;
        backgroundColor.color = canUse ? GetBackgroundColor() : CantUseColor;
        abilityIcon.sprite = abilityData.icon;
    }

    public void HideOrShowIcon(bool shouldShow)
    {
        abilityIcon.enabled = shouldShow;
        backgroundColor.enabled = shouldShow;

        if (shouldShow)
        {
            backgroundColor.color = GetBackgroundColor();
        }
    }

    public virtual void ActivateHover(bool playSound = true)
    {
        GameManager.Instance.UIManager.GetBookMenu().SkillSwitch.GetComponent<SkillSwitch>().UpdatePopup(abilityData);
        if (playSound)
        {
            GameManager.Instance.AudioManager.PlaySound(AudioRef.ButtonHover);
        }
        PlayHoverAnimation();
    }

    public virtual void DisableHover()
    {
        if (abilityData != null)
        {
            borderImage.sprite = bg;
        }
        else
        {
            borderImage.sprite = disabledBg;
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
            .SetUpdate(UpdateType.Normal, true)
            .Play();
    }

    public Color GetBackgroundColor()
    {
        switch (abilityData.elementType)
        {
            case ElementType.fire:
                Color fireColor = new Color(1f, 0.3f, 0.3f, 1f);
                return fireColor;
            case ElementType.water:
                Color waterColor = new Color(0.3f, 0.5f, 1f, 1f);
                return waterColor;
            case ElementType.wind:
                Color windColor = new Color(0.53f, 0.81f, 0.98f, 1f);
                return windColor;
            case ElementType.electric:
                Color electricColor = new Color(1f, 1f, 0.5f, 1f);
                return electricColor;
            default:
                return Color.white;
        }
    }
}
