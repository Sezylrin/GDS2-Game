using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    //Start,
    Hub,
    Tutorial
}

public class SceneLoader : MonoBehaviour
{
    public void Load(Scene scene, bool EnableDefaultTransition = true)
    {
        if (scene == Scene.Tutorial)
            GameManager.Instance.SetIsTutorial(true);
        if (EnableDefaultTransition)
            TriggerFade(scene);
        else
            SceneManager.LoadSceneAsync(scene.ToString());
    }

    public void LoadHub()
    {
        TriggerFade(Scene.Hub);
        GameManager.Instance.LevelGenerator.New();
    }

    public void LoadedIntoHub()
    {
        GameManager.Instance.PCM.system.FullHeal();
    }
    [field: Header("Default Transition")]
    [field: SerializeField]
    public Animator CrossFadeAnimator { get; private set; }
    [field:SerializeField]
    public float CrossFadeTime { get; private set; } = 1f;

    #region Default Transition
    public void TriggerFade(Scene scene)
    {
        StartCoroutine(TriggerCrossFadeStart(scene));
    }
    public IEnumerator TriggerCrossFadeStart(Scene scene)
    {
        CrossFadeAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(CrossFadeTime);
        SceneManager.LoadSceneAsync(scene.ToString());
        if (scene == Scene.Hub)
            LoadedIntoHub();
        CrossFadeAnimator.SetTrigger("End");
    }
    #endregion
}
