using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    [SerializeField] private SceneLoader loader;

    public void ReturnToHub()
    {
        loader.SeparateLoad(Scene.MainMenu);
    }
}
