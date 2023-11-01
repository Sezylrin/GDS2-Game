using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SliderControl
{
    Master,
    BGM,
    SFX,
}

public class SliderController : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] private Image SliderImage;
    [SerializeField] private RectTransform SliderRectTransform;
    [SerializeField] public SliderControl sliderControl;

    [Header("Images")]
    [SerializeField] private Sprite fiveMeter;
    [SerializeField] private Sprite fourMeter;
    [SerializeField] private Sprite threeMeter;
    [SerializeField] private Sprite twoMeter;
    [SerializeField] private Sprite oneMeter;
    [SerializeField] private Sprite zeroMeter;

    private int SliderValue = 5;
    private Tween hoverTween;

    public void SetSliderValue(float newSliderValue)
    {
        SliderValue = Mathf.RoundToInt(newSliderValue * 5 / 100);
        UpdateSliderImage();
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

    public void Increase()
    {
        if (SliderValue < 5)
        {
            SliderValue++;
            UpdateSliderImage();
            UpdateVolume();
            GameManager.Instance.AudioManager.PlaySound(AudioRef.buttonPress);
        }
    }

    public void Decrease()
    {
        if (SliderValue > 0)
        {
            SliderValue--;
            UpdateSliderImage();
            UpdateVolume();
            GameManager.Instance.AudioManager.PlaySound(AudioRef.buttonPress);
        }
    }

    private void UpdateSliderImage()
    {
        switch (SliderValue)
        {
            case 5:
                SliderImage.sprite = fiveMeter;
                break;
            case 4:
                SliderImage.sprite = fourMeter;
                break;
            case 3:
                SliderImage.sprite = threeMeter;
                break;
            case 2:
                SliderImage.sprite = twoMeter;
                break;
            case 1:
                SliderImage.sprite = oneMeter;
                break;
            case 0:
                SliderImage.sprite = zeroMeter;
                break;
        }
    }

    private void UpdateVolume()
    {
        float scaledVolume = (float)SliderValue / 5 * 100;

        switch (sliderControl)
        {
            case SliderControl.Master:
                GameManager.Instance.AudioManager.ModifyMasterVolume(scaledVolume);
                break;
            case SliderControl.BGM:
                GameManager.Instance.AudioManager.ModifyBGMVolume(scaledVolume);
                break;
            case SliderControl.SFX:
                GameManager.Instance.AudioManager.ModifySFXVolume(scaledVolume);
                break;
        }
    }

    protected void StopHoverAnimation()
    {
        hoverTween?.Kill();
        SliderRectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected void PlayHoverAnimation()
    {
        hoverTween?.Kill();
        hoverTween = DOTween.Sequence()
            .Append(SliderRectTransform.DOScale(new Vector3(1.05f, 1.05f, 1f), 0.3f).SetEase(Ease.InOutSine))
            .Append(SliderRectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(UpdateType.Normal, true)
            .Play();
    }
}
