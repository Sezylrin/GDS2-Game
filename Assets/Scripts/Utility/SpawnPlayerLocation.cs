using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerLocation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Player.Instance.transform.position = transform.position;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
