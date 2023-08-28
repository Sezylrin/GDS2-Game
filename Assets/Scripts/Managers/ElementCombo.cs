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
    [SerializeField]
    private LayerMask defaultMask;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttemptCombo(ElementType elementOne, ElementType elementTwo, IComboable comboInterface, LayerMask mask)
    {
        attemptedCombo = elementOne | elementTwo;
        switch ((int)attemptedCombo)
        {
            case (int)ElementCombos.aquaVolt:
                comboInterface.ApplyAquaVolt();
                break;
            case (int)ElementCombos.brambles:
                comboInterface.ApplyBrambles(mask);
                break;
            case (int)ElementCombos.fireSurge:
                comboInterface.ApplyFireSurge();
                break;
            case (int)ElementCombos.fireTornado:
                comboInterface.ApplyFireTornado(mask);
                break;
            case (int)ElementCombos.noxiousGas:
                comboInterface.ApplyNoxiousGas();
                break;
            case (int)ElementCombos.wither:
                comboInterface.ApplyWither();
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
