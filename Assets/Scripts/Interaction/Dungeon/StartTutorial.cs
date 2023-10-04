using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTutorial : InteractionBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Interact()
    {
        GameManager.Instance.sceneLoader.Load(Scene.Tutorial);
    }
}
