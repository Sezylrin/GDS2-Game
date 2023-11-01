using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseSkillTreeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] protected Image BackgroundColor;
    [SerializeField] protected Image Icon;

    protected int SoulCost;
    protected bool purchased = false;
    protected Color CantAffordColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Tween hoverTween;

    public virtual void Init()
    {

    }

    public virtual void ActivateHover()
    {
        PlayHoverAnimation();
        GameManager.Instance.AudioManager.PlaySound(AudioRef.ButtonHover);
    }

    public void DisableHover()
    {
        StopHoverAnimation();
    }

    public virtual void HandlePurchase()
    {

    }

    public virtual bool CanPurchase()
    {
        return GameManager.Instance.Souls >= SoulCost;
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
        HandlePurchase();
    }
}
