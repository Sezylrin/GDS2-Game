using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrieveSouls : InteractionBase
{
    
    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.Instance.IsSoulRetrieveRoom())
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        GameManager.Instance.RetrieveSouls();
        gameObject.SetActive(false);
    }
    
}
