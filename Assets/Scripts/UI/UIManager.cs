using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private GameObject activeMenu;

    [Header("Menus")]
    [SerializeField] private GameObject StartMenuContainer;
    [SerializeField] private GameObject BookMenuContainer;

    [Header("Container")]
    [SerializeField] private GameObject MenuContainer;

    public void OpenStartMenu()
    {
        StartMenuContainer.SetActive(true);
        activeMenu = StartMenuContainer;
        PlayOpenMenuSound();
        activeMenu.GetComponent<StartMenu>().Init();
    }

    public void OpenBookMenu()
    {
        BookMenuContainer.SetActive(true);
        activeMenu = BookMenuContainer;
    }

    public void EnableBookMenu()
    {
        GetBookMenu().IsInGame = true;
        activeMenu = BookMenuContainer;
    }

    public void CloseAll()
    {
        BookMenuContainer.GetComponent<BookMenu>().IsOpen = false;
        StartMenuContainer.SetActive(false);
        BookMenuContainer.SetActive(false);
    }

    #region Interactions
    public void Return(InputAction.CallbackContext context)
    {
        if (!activeMenu) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Return();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!activeMenu) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Interact();
    }

    public void Navigate(InputAction.CallbackContext context)
    {
        if (!activeMenu) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.Navigate(context);
    }

    public void ToggleSkills(InputAction.CallbackContext context)
    {
        if (!activeMenu) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.ToggleSkills();
    }

    public void ToggleOpenMenu(InputAction.CallbackContext context)
    {
        if (!GetBookMenu().IsInGame) return;

        Menu menu = activeMenu.GetComponent<Menu>();
        menu.ToggleOpenMenu();
    }

    public BookMenu GetBookMenu()
    {
        return BookMenuContainer.GetComponent<BookMenu>();
    }
    #endregion

    #region Sounds
    public void PlayOpenMenuSound()
    {
        GameManager.Instance.AudioManager.PlaySound(AudioRef.OpenMenu);
    }

    public void PlayCloseMenuSound()
    {
        GameManager.Instance.AudioManager.PlaySound(AudioRef.CloseMenu);
    }

    #endregion
}
