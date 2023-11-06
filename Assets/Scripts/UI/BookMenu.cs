using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening.Core.Easing;

public class BookMenu : Menu
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject Settings;
    [SerializeField] public GameObject SkillSwitch;
    [SerializeField] public GameObject SkillTree;

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
            GameManager.Instance.PlayerTransform.gameObject.SetActive(false);
            GameManager.Instance.UIManager.OpenBookMenu();
            IsOpen = true;
            MainMenu.SetActive(true);
            activeMenu = MainMenu;
            MainMenu.GetComponent<Menu>().OpenMenu();
            GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
            GameManager.Instance.MusicManager.ResumeMusic(AudioRef.PauseMenu, true);
            Time.timeScale = 0;
        }
    }

    public void ExitGame()
    {
        IsInGame = false;
        GameManager.Instance.EnemyManager.KillEnemies();
        GameManager.Instance.UIManager.CloseAll();
        GameManager.Instance.MusicManager.StopAllMusic();
        Time.timeScale = 1.0f;
        GameManager.Instance.sceneLoader.Load(Scene.MainMenu, false);
    }

    public void CloseMenu()
    {
        IsOpen = false;
        DisableAll();
        activeMenu = null;
        if (GameManager.Instance.IsTutorial || GameManager.Instance.LevelGenerator.isInCombatLevel)
        {
            GameManager.Instance.MusicManager.ResumeMultiple(new AudioRef[] { AudioRef.Combat, AudioRef.Grasslands }, true);
        }
        else
        {
            GameManager.Instance.MusicManager.ResumeMultiple(new AudioRef[] { AudioRef.Hub, AudioRef.Grasslands }, true);
        }
        GameManager.Instance.AudioManager.PlaySound(AudioRef.CloseMenu);
        Time.timeScale = 1.0f;
        GameManager.Instance.PlayerTransform.gameObject.SetActive(true);
    }

    public void OpenSettingsMenu()
    {
        if (activeMenu) activeMenu.SetActive(false);
        Settings.SetActive(true);
        activeMenu = Settings;
        activeMenu.GetComponent<Menu>().OpenMenu();
        GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
    }

    public void OpenSkillSwitch()
    {
        if (activeMenu) activeMenu.SetActive(false);
        SkillSwitch.SetActive(true);
        activeMenu = SkillSwitch;
        activeMenu.GetComponent<Menu>().OpenMenu();
        GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
    }

    public void OpenSkillTree()
    {
        if (activeMenu) activeMenu.SetActive(false);
        SkillTree.SetActive(true);
        activeMenu = SkillTree;
        activeMenu.GetComponent<Menu>().OpenMenu();
        GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
    }

    public void ReturnToMainMenu()
    {
        if (activeMenu) activeMenu.SetActive(false);
        MainMenu.SetActive(true);
        MainMenu.GetComponent<Menu>().OpenMenu();
        activeMenu = MainMenu;
        GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
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
