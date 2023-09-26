using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToHub : InteractionBase
{
    // Start is called before the first frame update
    public override void Interact()
    {
        GameManager.Instance.sceneLoader.Load(Scene.Hub);
    }

}
