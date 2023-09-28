using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
public enum EN_Scene
{
    Sprint2,
}

public static class Loader
{
    public static void Load(EN_Scene scene, bool EnableDefaultTransition)
    {
        SceneManager.LoadSceneAsync(scene.ToString());
    }
}