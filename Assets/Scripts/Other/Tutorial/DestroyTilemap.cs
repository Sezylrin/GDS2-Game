using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTilemap : MonoBehaviour
{
    public GameObject listenTo;
    public GameObject tilemap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!listenTo) Destroy();
    }

    public void Destroy()
    {
        Destroy(tilemap);
    }
}