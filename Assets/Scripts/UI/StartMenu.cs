using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : HorizontalMenu
{
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
        gameObject.SetActive(false);
    }
}
