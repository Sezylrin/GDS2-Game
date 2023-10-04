using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvents : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerSystem system))
        {
            system.Hitpoints -= 30;
            system.UpdateHealthUI();
            GameManager.Instance.PCM.system.AddToConsumeBar(100);
            Destroy(gameObject);
        }
    }
}