using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyRhinoHealth : MonoBehaviour
{
    public Rhino rhino;
    public int Amount;
    public int Damage;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SetIsTutorial(true);
    }

    // Update is called once per frame
    void Update()
    {
        rhino.SetOverRideHealth(Amount);
        // rhino.TakeDamage(Damage, 0, ElementType.noElement);
        Destroy(this);
    }
}