using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] SceneLoader loader;

    public void StartGame()
    {
        loader.Load(Scene.MainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {

    }
}
