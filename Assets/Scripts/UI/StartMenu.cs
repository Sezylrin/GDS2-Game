using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : HorizontalMenu
{
    [SerializeField] private GameObject MenuContainer;

    public void Init()
    {
        OpenMenu();
    }

    public void StartGame()
    {
        GameManager.Instance.sceneLoader.Load(Scene.Hub);
        GameManager.Instance.ActivateBookMenu();
        StartCoroutine(DeactivateMenuContainerAfterDelay());
    }

    private IEnumerator DeactivateMenuContainerAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        MenuContainer.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {

    }
}
