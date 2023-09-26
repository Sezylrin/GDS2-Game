using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    //Start,
    Hub
}

public class SceneLoader : MonoBehaviour
{
    public void Load(Scene scene, bool EnableDefaultTransition = true)
    {
        if (EnableDefaultTransition)
            GameManager.Instance.LevelGenerator.TriggerFade(scene);
        else
            SceneManager.LoadSceneAsync(scene.ToString());
    }

    public void LoadHub()
    {
        GameManager.Instance.LevelGenerator.TriggerFade(Scene.Hub);
    }

    public void LoadedIntoHub()
    {
        GameManager.Instance.PCM.system.FullHeal();
    }
}
