using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenu,
    Hub,
    Tutorial
}

public class SceneLoader : MonoBehaviour
{
    public void Load(Scene scene, bool EnableDefaultTransition = true)
    {
        if (scene == Scene.Tutorial)
        {
            GameManager.Instance.SetIsTutorial(true);
            GameManager.Instance.PCM.UI.DisableAbilityUI();
        }
        else
        {
            GameManager.Instance.SetIsTutorial(false);
        }
        if (EnableDefaultTransition)
            TriggerFade(scene);
        else
        {
            if (sceneTransition == null)
                sceneTransition = StartCoroutine(NoTransitionLoad(scene));
        }
    }

    public IEnumerator NoTransitionLoad(Scene scene)
    {
        var temp = SceneManager.LoadSceneAsync(scene.ToString());
        while (!temp.isDone)
        {
            yield return null;
        }
        sceneTransition = null;
    }

    public void LoadHub()
    {
        TriggerFade(Scene.Hub);
        GameManager.Instance.LevelGenerator.New();
        GameManager.Instance.SetIsTutorial(false);
    }

    public void LoadedIntoHub()
    {
        Debug.Log("Unhide");
        GameManager.Instance.PCM.system.FullHeal();
        GameManager.Instance.PlayerTransform.gameObject.SetActive(true);
        GameManager.Instance.PCM.control.RemoveBufferInput();        
    }
    [field: Header("Default Transition")]
    [field: SerializeField]
    public Animator CrossFadeAnimator { get; private set; }
    [field:SerializeField]
    public float CrossFadeTime { get; private set; } = 1f;
    public bool isFade { get; private set; }

    #region Default Transition
    private Coroutine sceneTransition;
    public void TriggerFade(Scene scene)
    {
        if (sceneTransition == null)
            sceneTransition = StartCoroutine(TriggerCrossFadeStart(scene));
    }
    public IEnumerator TriggerCrossFadeStart(Scene scene)
    {
        Debug.Log("called");
        CrossFadeAnimator.Play("Start");
        while (!isFade)
        {
            yield return null;
        }
        AsyncOperation temp = SceneManager.LoadSceneAsync((int)scene);
        float time = 0;
        while (!temp.isDone || !isFade || time < CrossFadeTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        if (scene == Scene.Hub)
            LoadedIntoHub();
        CrossFadeAnimator.Play("End");
        sceneTransition = null;
    }

    public void SetFade(int Fade)
    {
        isFade = Fade == 0 ? false: true ;
    }
    #endregion
}
