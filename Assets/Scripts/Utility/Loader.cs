using UnityEngine.SceneManagement;
using UnityEngine;
public enum EN_Scene
{
    Sprint2,
}

public static class Loader
{
    public static void Load(EN_Scene scene)
    {
        SceneManager.LoadSceneAsync(scene.ToString());
        GameManager.Instance.LevelGenerator.TriggerFade();
    }
}