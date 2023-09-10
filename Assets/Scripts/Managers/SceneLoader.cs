using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    Abilities,
    EnemyTest,
    LoadScene,
}

public class SceneLoader : MonoBehaviour
{
    private Dictionary<string, AsyncOperation> loadedScenes = new Dictionary<string, AsyncOperation>();

    public void PreloadScene(Scene scene)
    {
        string sceneName = scene.ToString();

        if (loadedScenes.ContainsKey(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is already preloaded.");
            return;
        }

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                loadedScenes[sceneName] = asyncOperation;
                break;
            }

            yield return null;
        }
    }

    public void SwitchToPreloadedScene(Scene scene)
    {
        string sceneName = scene.ToString();
        if (loadedScenes.ContainsKey(sceneName))
        {
            loadedScenes[sceneName].allowSceneActivation = true;
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' has not been preloaded.");
        }
    }

    public void UnloadPreloadedScene(Scene scene)
    {
        string sceneName = scene.ToString();
        if (loadedScenes.ContainsKey(sceneName))
        {
            SceneManager.UnloadSceneAsync(sceneName);

            loadedScenes.Remove(sceneName);
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' was not preloaded and cannot be unloaded.");
        }
    }
}
