using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class StatsManager : MonoBehaviour
{
    [Header("Abilities")]
    [SerializeField] 
    private List<ElementalSO> lockedAbilities;
    [SerializeField] 
    private List<ElementalSO> unlockedAbilities;
    [SerializeField]
    private ElementalSO[] startingEquippedAbilities = new ElementalSO[0];
    [SerializedDictionary("slot", "SO")]
    public SerializedDictionary<int, ElementalSO> savedAbilityPositions;

    [Header("Stats")]
    public int abilityModifier = 0;
    public int attackDamageModifier = 0;
    public int bonusHealth = 0;
    public bool secondSkillsetUnlocked = false;

    private void Awake()
    {
        if (savedAbilityPositions.Count == 0)
        {
            ResetEquipForTutorial();
        }
    }

    public void ResetEquipForTutorial()
    {
        savedAbilityPositions.Clear();
        for (int i = 0; i < startingEquippedAbilities.Length; i++)
        {
            savedAbilityPositions.Add(i, startingEquippedAbilities[i]);
        }
    }

    public void SetEquippedSkill()
    {
        for (int i = 0; i < startingEquippedAbilities.Length; i++)
        {
            savedAbilityPositions.Add(i, startingEquippedAbilities[i]);
        }
    }

    public void UnlockAbility(ElementalSO abilityToUnlock)
    {
        if (lockedAbilities.Contains(abilityToUnlock))
        {
            lockedAbilities.Remove(abilityToUnlock);
            unlockedAbilities.Add(abilityToUnlock);
        }
        else
        {
            Debug.LogWarning("Ability not found in locked abilities list!");
        }
    }

    public List<ElementalSO> GetLockedAbilities()
    {
        return lockedAbilities;
    }

    public List<ElementalSO> GetUnlockedAbilities()
    {
        return unlockedAbilities;
    }
}
