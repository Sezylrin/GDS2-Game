using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyBoundary : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> boundaries = new List<GameObject>();
    public List<GameObject> grids = new List<GameObject>();
    private int slot = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableBoundary()
    {
        boundaries[slot].SetActive(false);
        grids[slot].SetActive(false);
        slot++;
    }
}
