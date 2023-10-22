using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Vector3[] hitboxPos = new Vector3[4];
    [SerializeField]
    private Vector3[] hitboxRot = new Vector3[4];
    [SerializeField]
    private EnemyAnimation anim;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition= hitboxPos[anim.lastDir];
        transform.localEulerAngles = hitboxRot[anim.lastDir];
    }
}
