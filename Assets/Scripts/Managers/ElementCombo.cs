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
    //poison = 1<<4,
    //nature = 1<<5
}
public enum Combos
{
    fireSurge = ElementType.fire | ElementType.electric,
    steamExplosion = ElementType.fire | ElementType.water,
    fireTornado = ElementType.fire | ElementType.wind,
    shock = ElementType.electric | ElementType.water,
    zap = ElementType.electric | ElementType.wind,
    blizzard = ElementType.water | ElementType.wind
}
public class ElementCombo : MonoBehaviour
{    
    
    [SerializeField, ReadOnly]
    private Combos attemptedCombo;
    public ElementType ElementOne;
    public ElementType ElementTwo;
    public IComboable test;
    [SerializeField]
    private LayerMask defaultMask;
    [SerializeField][SerializedDictionary("Element Combo", "ComboSo")]
    private SerializedDictionary<Combos, ComboSO> setCombos;

    private Pool<ComboBase> tornadoPool;
    private Pool<ComboBase> blizzardPool;
    [SerializeField]
    private GameObject fireTornado;
    [SerializeField]
    private GameObject blizzard;

    void Start()
    {
        GameManager.Instance.PoolingManager.FindPool(fireTornado, out tornadoPool);
        GameManager.Instance.PoolingManager.FindPool(blizzard, out blizzardPool);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttemptCombo(ElementType elementOne, ElementType elementTwo, Enemy enemy, LayerMask mask, int comboTier, Vector3 pos)
    {
        attemptedCombo = (Combos)(elementOne | elementTwo);
        ComboSO combo = null;
        setCombos.TryGetValue(attemptedCombo, out combo);
        switch ((int)attemptedCombo)
        {
            case (int)Combos.fireSurge:
                enemy.ComboAttack(combo, elementOne, elementTwo, Combos.fireSurge);
                break;
            case (int)Combos.steamExplosion:
                enemy.ComboAttack(combo, elementOne, elementTwo, Combos.steamExplosion);
                break;
            case (int)Combos.zap:
                enemy.ComboAttack(combo, elementOne, elementTwo, Combos.zap);
                break;
            case (int)Combos.shock:
                enemy.ComboAttack(combo, elementOne, elementTwo, Combos.shock);
                ShockSO shock = combo as ShockSO;
                StartCoroutine(enemy.StunTarget(shock.duration));
                break;
            case (int)Combos.fireTornado:
                SpawnAreaCombo(tornadoPool,combo, mask, pos);
                break;
            case (int)Combos.blizzard:
                SpawnAreaCombo(blizzardPool, combo, mask, pos);
                break;
            /*case (int)Combos.aquaVolt:
                comboInterface.ApplyAquaVolt(combo.BaseDamage[comboTier], combo.StaggerDamage[comboTier], combo.Duration[comboTier]);
                break;
            case (int)Combos.brambles:
                SpawnBramble(combo, comboTier, mask, pos);
                break;
            case (int)Combos.fireSurge:
                comboInterface.ApplyFireSurge(combo.BaseDamage[comboTier], combo.StaggerDamage[comboTier]);
                break;
            case (int)Combos.fireTornado:
                SpawnFireTornado(combo, comboTier, mask, pos);
                break;
            case (int)Combos.noxiousGas:
                comboInterface.ApplyNoxiousGas(combo.BaseDamage[comboTier], combo.StaggerDamage[comboTier], combo.Duration[comboTier]);
                break;
            case (int)Combos.wither:
                comboInterface.ApplyWither(combo.BaseDamage[comboTier], combo.StaggerDamage[comboTier], combo.Duration[comboTier], (combo as WitherSO).WitherStrength[comboTier]);
                break;*/
        }
    }

    [ContextMenu("test1")]
    public void test1()
    {
        attemptedCombo = (Combos)(ElementOne | ElementTwo);

        ComboSO combo = null;
        setCombos.TryGetValue(attemptedCombo, out combo);
        Debug.Log("testing " + combo);
        AttemptCombo(ElementOne, ElementTwo, new Rhino(), defaultMask, 0, transform.position);
    }

    private void SpawnAreaCombo(Pool<ComboBase> pool ,ComboSO combo, LayerMask target, Vector3 pos)
    {
        bool newSpawn;
        ComboBase temp = pool.GetPooledObj(out newSpawn);
        if (newSpawn)
        {
            temp.InitSpawn();
        }
        temp.Init(combo as AreaComboSO, pos, target);
    }

}
