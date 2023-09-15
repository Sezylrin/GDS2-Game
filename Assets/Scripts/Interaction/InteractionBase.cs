using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InteractionBase : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    protected GameObject UI;
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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.T_Player))
        {
            GameManager.Instance.SetInteraction(this);
            if (UI)
                UI.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.T_Player))
        {
            GameManager.Instance.RemoveInteraction(this);
            if (UI)
                UI.SetActive(false);
        }
    }
}
