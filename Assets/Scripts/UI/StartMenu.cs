using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.sceneLoader.Load(Scene.Hub);
        GameManager.Instance.ActivateBookMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {

    }
}
