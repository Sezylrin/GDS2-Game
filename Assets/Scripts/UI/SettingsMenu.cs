using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : Menu
{
    [SerializeField] private BookButton ReturnButton;
    [SerializeField] private SliderController MasterController;
    [SerializeField] private SliderController SFXController;
    [SerializeField] private SliderController BGMController;

    [SerializeField] private bool IsMainMenu;

    private float navigationCooldownDuration = 0.15f;
    private bool isOnCooldown = false;

    private SliderController ActiveSlider;
    private bool ReturnButtonActive = false;

    public override void OpenMenu()
    {
        AudioManager audioManager = GameManager.Instance.AudioManager;
        MasterController.SetSliderValue(audioManager.masterVolume);
        SFXController.SetSliderValue(audioManager.sfxVolume);
        BGMController.SetSliderValue(audioManager.bgmVolume);
        MasterController.ActivateHover();
        ActiveSlider = MasterController;
        ReturnButtonActive = false;
    }

    #region Interactions
    public override void Return()
    {
        if (IsMainMenu)
        {
            GameManager.Instance.UIManager.GetStartMenu().CloseSettings();
        }
        else
        {
            GameManager.Instance.UIManager.GetBookMenu().ReturnToMainMenu();
        }
    }

    public override void ToggleOpenMenu()
    {
        if (IsMainMenu)
        {
            GameManager.Instance.UIManager.GetStartMenu().CloseSettings();
        }
        else
        {
            GameManager.Instance.UIManager.GetBookMenu().CloseMenu();
        }
    }

    public override void Interact()
    {
        if (!ReturnButtonActive) return;

        ReturnButton.HandleClick();
    }
    #endregion

    #region ButtonClicks
    public void IncreaseMasterVolume()
    {
        MasterController.Increase();
    }

    public void DecreaseMasterVolume()
    {
        MasterController.Decrease();
    }

    public void IncreaseSFXVolume()
    {
        SFXController.Increase();
    }

    public void DecreaseSFXVolume() 
    {
        SFXController.Decrease();
    }

    public void IncreaseBGMVolume()
    {
        BGMController.Increase();
    }

    public void DecreaseBGMVolume()
    {
        BGMController.Decrease();
    }
    #endregion

    #region Navigation
    public override void Navigate(InputAction.CallbackContext context)
    {
        if (!isOnCooldown)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            if (direction.y > 0.5f)
                NavigateUp();
            else if (direction.y < -0.5f)
                NavigateDown();
            else if (direction.x < -0.5)
                NavigateLeft();
            else if (direction.x > 0.5)
                NavigateRight();

            StartCoroutine(NavigationCooldown());
        }
    }

    private void NavigateUp()
    {
        if (ReturnButtonActive)
        {
            ActiveSlider = BGMController;
            BGMController.ActivateHover();
            ReturnButtonActive = false;
            ReturnButton.DisableHover();
        }
        else
        {
            switch (ActiveSlider.sliderControl)
            {
                case SliderControl.Master:
                    ActiveSlider = null;
                    MasterController.DisableHover();
                    ReturnButtonActive = true;
                    ReturnButton.ActivateHover();
                    break;
                case SliderControl.SFX:
                    ActiveSlider = MasterController;
                    SFXController.DisableHover();
                    MasterController.ActivateHover();
                    break;
                case SliderControl.BGM:
                    ActiveSlider = SFXController;
                    BGMController.DisableHover();
                    SFXController.ActivateHover();
                    break;
            }
        }
    }

    private void NavigateDown()
    {
        if (ReturnButtonActive)
        {
            ActiveSlider = MasterController;
            MasterController.ActivateHover();
            ReturnButtonActive = false;
            ReturnButton.DisableHover();
        }
        else
        {
            switch (ActiveSlider.sliderControl)
            {
                case SliderControl.Master:
                    ActiveSlider = SFXController;
                    MasterController.DisableHover();
                    SFXController.ActivateHover();
                    break;
                case SliderControl.SFX:
                    ActiveSlider = BGMController;
                    SFXController.DisableHover();
                    BGMController.ActivateHover();
                    break;
                case SliderControl.BGM:
                    ActiveSlider = null;
                    BGMController.DisableHover();
                    ReturnButtonActive = true;
                    ReturnButton.ActivateHover();
                    break;
            }
        }
    }

    private void NavigateLeft()
    {
        if (ReturnButtonActive) return;

        ActiveSlider.Decrease();
    }

    private void NavigateRight()
    {
        if (ReturnButtonActive) return;

        ActiveSlider.Increase();
    }

    private IEnumerator NavigationCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSecondsRealtime(navigationCooldownDuration);
        isOnCooldown = false;
    }
    #endregion
}
