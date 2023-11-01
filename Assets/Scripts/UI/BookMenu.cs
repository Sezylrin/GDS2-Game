using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;

public class BookMenu : Menu
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject Settings;
    [SerializeField] public GameObject SkillSwitch;
    [SerializeField] private GameObject SkillTree;

    private GameObject activeMenu;
    public bool IsOpen = false;
    public bool IsInGame = false;

    #region Interactions
    public override void ToggleOpenMenu()
    {
        if (IsInGame)
        {
            ToggleMenu();
        }
    }

    public void DisableAll()
    {
        MainMenu.SetActive(false);
        Settings.SetActive(false);
        SkillSwitch.SetActive(false);
        SkillTree.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (IsOpen)
        {
            CloseMenu();
        }
        else
        {
            GameManager.Instance.UIManager.OpenBookMenu();
            IsOpen = true;
            MainMenu.SetActive(true);
            activeMenu = MainMenu;
            MainMenu.GetComponent<Menu>().OpenMenu();
            GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
            Time.timeScale = 0;
        }
    }

    public void ExitGame()
    {
        GameManager.Instance.sceneLoader.Load(Scene.MainMenu, false);
        IsInGame = false;
        GameManager.Instance.UIManager.CloseAll();
    }

    public void CloseMenu()
    {
        IsOpen = false;
        DisableAll();
        activeMenu = null;
        GameManager.Instance.AudioManager.PlaySound(AudioRef.CloseMenu);
        Time.timeScale = 1.0f;
    }

    public void OpenSettingsMenu()
    {
        if (activeMenu) activeMenu.SetActive(false);
        Settings.SetActive(true);
        activeMenu = Settings;
        activeMenu.GetComponent<Menu>().OpenMenu();
    }

    public void OpenSkillSwitch()
    {
        if (activeMenu) activeMenu.SetActive(false);
        SkillSwitch.SetActive(true);
        activeMenu = SkillSwitch;
        activeMenu.GetComponent<Menu>().OpenMenu();
    }

    public void ReturnToMainMenu()
    {
        if (activeMenu) activeMenu.SetActive(false);
        MainMenu.SetActive(true);
        MainMenu.GetComponent<Menu>().OpenMenu();
        activeMenu = MainMenu;
    }

    public override void Return()
    {
        if (!IsOpen) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Return();
    }

    public override void Interact()
    {
        if (!IsOpen) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Interact();
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        if (!IsOpen) return;
        
        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Navigate(context);
    }

    public override void ToggleSkills()
    {
        if (!IsOpen) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.ToggleSkills();
    }
    #endregion
}
