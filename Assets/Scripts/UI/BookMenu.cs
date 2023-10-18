using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class BookMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject Settings;
    [SerializeField] public GameObject SkillSwitch;
    [SerializeField] private GameObject SkillTree;

    private GameObject activeMenu;
    private bool IsOpen = false;

    #region Interactions
    public void ToggleOpenMenu(InputAction.CallbackContext context)
    {
        ToggleMenu();
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
            IsOpen = false;
            DisableAll();
            activeMenu = null;
            GameManager.Instance.AudioManager.PlaySound(AudioRef.CloseMenu);
            Time.timeScale = 1.0f;
        }
        else
        {
            IsOpen = true;
            MainMenu.SetActive(true);
            activeMenu = MainMenu;
            GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
            Time.timeScale = 0;
        }
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
        activeMenu = MainMenu;
    }

    public void Return(InputAction.CallbackContext context)
    {
        if (!IsOpen) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Return();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!IsOpen) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Interact();
    }

    public void Navigate(InputAction.CallbackContext context)
    {
        if (!IsOpen) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Navigate(context);
    }

    public void ToggleSkills(InputAction.CallbackContext context)
    {
        if (!IsOpen) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.ToggleSkills();
    }
    #endregion
}
