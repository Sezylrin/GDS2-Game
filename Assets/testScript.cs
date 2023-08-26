using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public PolygonCollider2D col2D;
    // Start is called before the first frame update
    void Start()
    {
        Vector2[] temparray = { new Vector2(1, 1), new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, -1) };
        col2D.SetPath(0, temparray);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
