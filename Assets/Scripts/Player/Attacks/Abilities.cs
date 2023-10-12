using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using AYellowpaper.SerializedCollections;
using UnityEngine.InputSystem;


public class Abilities : MonoBehaviour
{
    [Header("Core")]
    [SerializeField]
    private PlayerComponentManager PCM;
    [SerializeField]
    private Transform abilitySpawnPoint;

    [Header("abilities")]
    [SerializeField]
    private ElementalSO[] abilities = new ElementalSO[8];
    [SerializeField][ReadOnly]
    private bool AbilitySetOne;

    private Dictionary<AbilityType, Pool<AbilityBase>> pools = new Dictionary<AbilityType, Pool<AbilityBase>>();

    private ElementalSO lastUsed;
    [SerializeField][SerializedDictionary("Element Type", "Description")]
    private SerializedDictionary<AbilityType, GameObject> abilityShapePF;
    private void Start()
    {
        foreach (KeyValuePair<AbilityType,GameObject> entry in abilityShapePF)
        {
            Pool<AbilityBase> temp;
            GameManager.Instance.PoolingManager.FindPool(entry.Value, out temp,entry.Key.ToString() + " type");
            pools.Add(entry.Key, temp);
        }
        //remember to uncheck them when done with debugging
        //SetAbilities();
        PCM.UI.UpdateAbilityText(GetName(0), GetName(1), GetName(2));
    }
    public void ToggleActiveAbilitySet(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.StatsManager.secondSkillsetUnlocked)
            return;
        AbilitySetOne = !AbilitySetOne;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (AbilitySetOne)
        {
            PCM.UI.UpdateAbilityText(GetName(3), GetName(4), GetName(5));
        }
        else
        {
            PCM.UI.UpdateAbilityText(GetName(0), GetName(1), GetName(2));
        }
    }

    private string GetName(int index)
    {
        if(abilities[index] == null)
        {
            return "";
        }
        else
        {
            return abilities[index].name + " " + abilities[index].castCost;
        }
    }
    public void CastSlotOne()
    {
        StartCast(AbilitySetOne ? abilities[4] : abilities[0]);
    }
    public void CastSlotTwo()
    {
        StartCast(AbilitySetOne ? abilities[5] : abilities[1]);
    }
    public void CastSlotThree()
    {
        StartCast(AbilitySetOne ? abilities[6] : abilities[2]);
    }
    public void CastSlotFour()
    {
        StartCast(AbilitySetOne ? abilities[7] : abilities[3]);
    }
    public bool IsRanged(int slot)
    {
        if (AbilitySetOne)
            slot += 3;
        if (!abilities[slot])
            return false;
        return (abilities[slot].type == AbilityType.Projectile);
    }
    public bool CanCast(int slot)
    {
        if(AbilitySetOne)
            slot += 3;
        return (PCM.system.CanCast(abilities[slot].castCost));
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
        UpdateUI();
    }
    private ElementalSO abilityToCast;
    private void StartCast(ElementalSO selected)
    {
        Debug.Log(selected.name);
        if (!selected)
            return;
        abilityToCast = selected;
        PCM.control.RemoveBufferInput();
        PCM.control.StartAbility(selected.castStartSpeed);
    }
    public void CastAbility(out float castDur)
    {
        lastUsed = abilityToCast;
        castDur = abilityToCast.castDuration;
        //play animation
        Pool<AbilityBase> temp;
        if (pools.TryGetValue(abilityToCast.type, out temp))
        {
            
            AbilityBase ability = temp.GetPooledObj(out bool initial);
            if (initial)
            {
                ability.init();
            }
            if (abilityToCast.type.Equals(AbilityType.AOE))
            {
                ability.SetSelectedAbility(abilityToCast, transform.position);
            }            
            else
            {
                Vector3 dir;
                if (abilityToCast.type.Equals(AbilityType.dash))
                    dir = PCM.control.lastDirection;
                else
                    dir = abilitySpawnPoint.position - transform.position;
                if (abilityToCast.type.Equals(AbilityType.blast))
                    PCM.system.AddForce(dir.normalized * 13);
                ability.SetSelectedAbility(abilityToCast, abilitySpawnPoint.position, dir, transform);
            }
        }
    }

    #region debug
    [SerializeField]
    private ElementalSO test;

    [ContextMenu("castAbility")]
    private void CastAbility()
    {
        CastAbility();
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


