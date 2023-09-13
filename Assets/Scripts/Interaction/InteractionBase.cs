using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.T_Player))
        {
            GameManager.Instance.SetInteraction(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.T_Player))
        {
            GameManager.Instance.RemoveInteraction(this);
        }
    }
}
