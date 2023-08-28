using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using AYellowpaper.SerializedCollections;

namespace AYellowpaper.SerializedCollections
{
    [System.Serializable]
    public class AbilityDictionary
    {
        [SerializedDictionary("Element Type", "Description")]
        public SerializedDictionary<AbilityType, GameObject> abilityShapePF;
    }
}
public class Abilities : MonoBehaviour
{
    [Header("Core")]
    [SerializeField]
    private PlayerComponentManager PCM;
    [SerializeField]
    private Transform abilitySpawnPoint;

    [Header("abilities")]
    [SerializeField]
    [ReadOnly]
    private ElementalSO[] abilities = new ElementalSO[6];

    private bool AbilitySetOne;

    private Dictionary<AbilityType, Pool<AbilityBase>> pools = new Dictionary<AbilityType, Pool<AbilityBase>>();

    [SerializeField]
    private AbilityDictionary abilityShape;
    private void Start()
    {
        foreach (KeyValuePair<AbilityType,GameObject> entry in abilityShape.abilityShapePF)
        {
            Pool<AbilityBase> temp;
            PoolingManager.Instance.FindPool(entry.Value, out temp,entry.Key.ToString() + " type");
            pools.Add(entry.Key, temp);
        }
        //remember to uncheck them when done with debugging
        SetAbilities();
    }
    public void CastSlotOne()
    {
        PCM.control.RemoveBufferInput();
        CastAbility(AbilitySetOne ? abilities[3] : abilities[0]);
    }
    public void CastSlotTwo()
    {
        PCM.control.RemoveBufferInput();
        CastAbility(AbilitySetOne ? abilities[4] : abilities[1]);
    }
    public void CastSlotThree()
    {
        PCM.control.RemoveBufferInput();
        CastAbility(AbilitySetOne ? abilities[5] : abilities[2]);
    }
    public void SetSlot(ElementalSO abilityToUse, int slot)
    {
        if (slot < 0 || slot > 6)
        {
            Debug.Break();
            Debug.Log("Invalid Slot number");
        }
        if (!abilityToUse)
            return;
        abilities[slot] = abilityToUse;
    }

    private void CastAbility(ElementalSO selected)
    {
        if (!selected)
            return;
        if (!PCM.system.AttemptCast(selected.castCost))
            return;
        //play animation
        Pool<AbilityBase> temp;
        if (pools.TryGetValue(selected.type, out temp))
        {
            AbilityBase ability = temp.GetPooledObj();
            if (selected.type.Equals(AbilityType.AOE))
            {
                ability.SetSelectedAbility(selected, transform.position);
            }
            else
            {
                Vector3 dir = abilitySpawnPoint.position - transform.position;
                ability.SetSelectedAbility(selected, abilitySpawnPoint.position, dir);
            }
            
        }
    }

    #region debug
    [SerializeField]
    private ElementalSO test;

    [ContextMenu("castAbility")]
    private void CastAbility()
    {
        CastAbility(test);
    }

    [ContextMenu("SetAbilitiesToDebug")]
    private void SetAbilities()
    {
        for (int i = 0; i < 6; i++)
        {
            SetSlot(test, i);
        }
    }
    #endregion
}

