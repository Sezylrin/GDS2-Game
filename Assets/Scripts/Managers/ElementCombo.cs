using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType : byte
{
    fire = 1,
    water = 2,
    electric = 4,
    wind = 8,
    poison = 16,
    nature = 32,
    noElement = 64
}

public enum ElementCombos : byte
{
    fireSurge = 6,
    aquaVolt = 5,
    fireTornado = 9,
    toxicGas = 24,
    brambles = 34,
    poisonPlant = 48
}
public class ElementCombo : MonoBehaviour
{
    // Start is called before the first frame update
    public ElementCombos currentCombo;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("test1")]
    public void test1()
    {
        currentCombo = (ElementCombos)((byte)ElementType.water + ElementType.nature);
    }
}
