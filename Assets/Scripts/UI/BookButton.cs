using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using DG.Tweening.Core.Easing;
using UnityEngine.UI;

public class BookButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Tween hoverTween;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void ActivateHover(bool PlaySound = true)
    {
        PlayHoverAnimation();
        if (PlaySound) GameManager.Instance.AudioManager.PlaySound(AudioRef.ButtonHover);
    }

    public void DisableHover()
    {
        StopHoverAnimation();
    }

    public void HandleClick()
    {
        GameManager.Instance.AudioManager.PlaySound(AudioRef.buttonPress);
        StopHoverAnimation();
        button.onClick.Invoke();
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
        HandleClick();
    }
}
