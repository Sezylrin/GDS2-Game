using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartMenu : Menu
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainMenu;

    [SerializeField]
    BookButton[] buttons;

    private int currentSelectedIndex = 0;
    private bool SettingsOpen = false;
    private float navigationCooldownDuration = 0.15f;
    private bool isOnCooldown = false;

    public override void OpenMenu()
    {
        SetActiveButton(0);
    }

    public void SetActiveButton(int index)
    {
        if (index < 0)
            index = buttons.Length - 1;
        else if (index >= buttons.Length)
            index = 0;

        buttons[currentSelectedIndex].DisableHover();

        currentSelectedIndex = index;

        buttons[currentSelectedIndex].ActivateHover();
    }

    public void ClickActiveButton()
    {
        buttons[currentSelectedIndex].HandleClick();
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        if (!SettingsOpen)
        {
            if (!isOnCooldown)
            {
                Vector2 direction = context.ReadValue<Vector2>();
                if (direction.y > 0.5f)
                    SetActiveButton(currentSelectedIndex - 1);
                else if (direction.y < -0.5f)
                    SetActiveButton(currentSelectedIndex + 1);

                StartCoroutine(NavigationCooldown());
            }
        }
        else
        {
            settingsMenu.GetComponent<Menu>().Navigate(context);
        }
    }

    public override void Interact()
    {
        if (!SettingsOpen) {
            ClickActiveButton();
        }
        else
        {
            settingsMenu.GetComponent<Menu>().Interact();
        }
    }

    public override void ToggleOpenMenu()
    {
        if (SettingsOpen)
        {
            settingsMenu.GetComponent<Menu>().ToggleOpenMenu();
        }
    }

    private IEnumerator NavigationCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSecondsRealtime(navigationCooldownDuration);
        isOnCooldown = false;
    }
    public void Init()
    {
        OpenMenu();
    }

    public void StartGame()
    {
        GameManager.Instance.sceneLoader.Load(Scene.Hub);
        GameManager.Instance.UIManager.EnableBookMenu();
        StartCoroutine(DeactivateMenuContainerAfterDelay());
    }

    private IEnumerator DeactivateMenuContainerAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        GameManager.Instance.UIManager.CloseAll();
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

    public void OpenSettings()
    {
        SettingsOpen = true;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<SettingsMenu>().OpenMenu();
    }

    public void CloseSettings()
    {
        SettingsOpen = false;
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
        OpenMenu();
    }
}
