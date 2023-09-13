using UnityEngine.SceneManagement;
public enum EN_Scene
{
    Game,
}

public static class Loader
{
    public static void Load(EN_Scene scene)
    {
        SceneManager.LoadSceneAsync(scene.ToString());
    }
}