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
        PCM.UI.UpdateAbilityText(GetName(0), GetName(1), GetName(2), GetName(3));
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
            PCM.UI.UpdateAbilityText(GetName(4), GetName(5), GetName(6), GetName(7));
        }
        else
        {
            PCM.UI.UpdateAbilityText(GetName(0), GetName(1), GetName(2),GetName(3));
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
            return abilities[index].name + " ";
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
        if (!selected)
            return;
        abilityToCast = selected;
        PCM.control.RemoveBufferInput();
        PCM.control.StartAbility(selected.castStartSpeed);
        castDir = abilitySpawnPoint.position - transform.position;
        castType = selected.type;
    }
    public Vector2 castDir { get; private set; }
    public AbilityType castType { get; private set; }
    public void CastAbility(out float castDur)
    {
        lastUsed = abilityToCast;
        castDur = abilityToCast.castDuration;
        GameManager.Instance.AudioManager.PlaySound(abilityToCast.audioCast);
        //play animation
        Pool<AbilityBase> temp;
        if (pools.TryGetValue(abilityToCast.type, out temp))
        {
            
            AbilityBase ability = temp.GetPooledObj(out bool initial);
            GameManager.Instance.EnemyManager.UpdateAttacksList(abilityToCast.elementType);
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
                if (abilityToCast.type.Equals(AbilityType.blast))
                    PCM.system.AddForce(castDir.normalized * 13);
                ability.SetSelectedAbility(abilityToCast, abilitySpawnPoint.position, castDir, transform);
            }
        }
    }

    public ElementalSO[] GetAbilities()
    {
        return abilities;
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


