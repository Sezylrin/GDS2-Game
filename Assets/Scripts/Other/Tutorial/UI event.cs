using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIevent : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerUI system))
        {
            system.EnableAbilityUI();
            Destroy(gameObject);
        }
    }
}
