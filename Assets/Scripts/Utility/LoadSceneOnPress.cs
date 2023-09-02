using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneOnPress : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.Instance.sceneLoader.PreloadScene(Scene.EnemyTest);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameManager.Instance.sceneLoader.PreloadScene(Scene.Abilities);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameManager.Instance.sceneLoader.SwitchToPreloadedScene(Scene.EnemyTest);
            Debug.Log("hi");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Bye");
            GameManager.Instance.sceneLoader.SwitchToPreloadedScene(Scene.Abilities);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameManager.Instance.sceneLoader.UnloadPreloadedScene(Scene.EnemyTest);
        }
    }
}
