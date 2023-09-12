using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using AYellowpaper.SerializedCollections;

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
    fireSurge,
    aquaVolt,
    fireTornado,
    noxiousGas,
    brambles,
    wither
}


public class ElementCombo : MonoBehaviour
{    
    private enum Combos
    {
        fireSurge = ElementType.fire | ElementType.electric,
        aquaVolt = ElementType.water | ElementType.electric,
        fireTornado = ElementType.fire | ElementType.wind,
        noxiousGas = ElementType.poison | ElementType.wind,
        brambles = ElementType.nature | ElementType.water,
        wither = ElementType.nature | ElementType.poison
    }
    [SerializeField, ReadOnly]
    private Combos attemptedCombo;
    public ElementType ElementOne;
    public ElementType ElementTwo;
    public IComboable test;
    [SerializeField]
    private LayerMask defaultMask;
    [SerializeField][SerializedDictionary("Element Combo", "ComboSo")]
    private SerializedDictionary<Combos, ComboSO> setCombos;

    private Pool<ComboBase> pool;
    [SerializeField]
    private GameObject testobj;

    void Start()
    {
        GameManager.Instance.PoolingManager.FindPool(testobj, out pool);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttemptCombo(ElementType elementOne, ElementType elementTwo, IComboable comboInterface, LayerMask mask, int comboTier, Vector3 pos)
    {
        attemptedCombo = (Combos)(elementOne | elementTwo);
        ComboSO combo = null;
        setCombos.TryGetValue(attemptedCombo, out combo);
        switch ((int)attemptedCombo)
        {
            case (int)Combos.aquaVolt:
                comboInterface.ApplyAquaVolt(combo.BaseDamage[comboTier], combo.StaggerDamage, combo.Duration);
                break;
            case (int)Combos.brambles:
                break;
            case (int)Combos.fireSurge:
                comboInterface.ApplyFireSurge(combo.BaseDamage[comboTier], combo.StaggerDamage);
                break;
            case (int)Combos.fireTornado:
                break;
            case (int)Combos.noxiousGas:
                comboInterface.ApplyNoxiousGas(combo.BaseDamage[comboTier], combo.StaggerDamage, combo.Duration);
                break;
            case (int)Combos.wither:
                comboInterface.ApplyWither(combo.BaseDamage[comboTier], combo.StaggerDamage, combo.Duration, (combo as WitherSO).WitherStrength[comboTier]);
                break;
        }
    }

    [ContextMenu("test1")]
    public void test1()
    {
        attemptedCombo = (Combos)(ElementOne | ElementTwo);

        ComboSO combo = null;
        setCombos.TryGetValue(attemptedCombo, out combo);
        Debug.Log("testing " + combo);
        AttemptCombo(ElementOne, ElementTwo, test, defaultMask, 0, transform.position);
    }

    private void SpawnFireTornado(ComboSO combo, int tier, LayerMask target)
    {
        bool newSpawn;
        ComboBase temp = pool.GetPooledObj(out newSpawn);
        if (newSpawn)
        {
            temp.InitSpawn();
        }
        temp.Init(combo as AreaComboSO, tier, Vector3.zero, target);
        Debug.Break();
    }
}
