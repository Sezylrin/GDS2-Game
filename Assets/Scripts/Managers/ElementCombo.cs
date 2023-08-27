using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public enum ElementType
{
    noElement = 0,
    fire = 1<<0,
    water = 1<<1,
    electric = 1<<2,
    wind = 1<<3,
    poison = 1<<4,
    nature = 1<<5
}

public enum ElementCombos
{
    fireSurge = ElementType.fire | ElementType.electric,
    aquaVolt = ElementType.water | ElementType.electric,
    fireTornado = ElementType.fire | ElementType.wind,
    noxiousGas = ElementType.poison | ElementType.wind,
    brambles = ElementType.nature | ElementType.water,
    wither = ElementType.nature | ElementType.poison
}
public class ElementCombo : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private ElementCombos currentCombo;
    [SerializeField, ReadOnly]
    private ElementType attemptedCombo;
    public ElementType ElementOne;
    public ElementType ElementTwo;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttemptCombo(ElementType elementOne, ElementType elementTwo)
    {
        attemptedCombo = elementOne | elementTwo;
        switch ((int)attemptedCombo)
        {
            case (int)ElementCombos.aquaVolt:
                break;
            case (int)ElementCombos.brambles:
                break;
            case (int)ElementCombos.fireSurge:
                break;
            case (int)ElementCombos.fireTornado:
                break;
            case (int)ElementCombos.noxiousGas:
                break;
            case (int)ElementCombos.wither:
                break;
        }
    }

    [ContextMenu("test1")]
    public void test1()
    {
        attemptedCombo = ElementOne | ElementTwo;
        Debug.Log((int)attemptedCombo);
    }
}
