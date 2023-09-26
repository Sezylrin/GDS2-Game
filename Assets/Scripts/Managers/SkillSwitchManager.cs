using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillSwitchManager : MonoBehaviour
{
    [SerializeField]
    private GameObject allAbilitiesContainer;
    [SerializeField]
    private GameObject activeAbilitiesContainer;

    private List<GameObject> allAbilities = new();
    private List<GameObject> activeAbilities = new();

    private UnusedAbility currentlyHoveredAbility;
    private int currentlyHoveredIndex = 0;

    private float navigationCooldown = 0.15f; 
    private float lastNavigationTime;

    private void Awake()
    {
        StatsManager statsManager = GameManager.Instance.StatsManager;
        List<ElementalSO> unlockedAbilities = statsManager.GetUnlockedAbilities();
        List<ElementalSO> lockedAbilities = statsManager.GetLockedAbilities();

        int abilityIndex = 0;

        for (int i = 0; i < allAbilitiesContainer.transform.childCount; i++)
        {
            Transform childTransform = allAbilitiesContainer.transform.GetChild(i);
            UnusedAbility unusedAbility = childTransform.gameObject.GetComponent<UnusedAbility>();
            if (unusedAbility != null)
            {
                if (abilityIndex < unlockedAbilities.Count)
                {
                    unusedAbility.abilityData = unlockedAbilities[abilityIndex];
                }
                else if (abilityIndex - unlockedAbilities.Count < lockedAbilities.Count)
                {
                    unusedAbility.abilityData = lockedAbilities[abilityIndex - unlockedAbilities.Count];
                }
                abilityIndex++;
            }
            childTransform.GetChild(0).GetComponent<Image>().sprite = unusedAbility.abilityData.icon;
            allAbilities.Add(childTransform.gameObject);
        }

        for (int i = 0; i < activeAbilitiesContainer.transform.childCount; i++)
        {
            Transform childTransform = activeAbilitiesContainer.transform.GetChild(i);
            activeAbilities.Add(childTransform.gameObject);
        }

        UnusedAbility firstAbility = allAbilities[0].GetComponent<UnusedAbility>();
        firstAbility.ActivateHover();
        currentlyHoveredAbility = firstAbility;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interacting");
    }

    public void ToggleSkills(InputAction.CallbackContext context)
    {
        Debug.Log("Toggling skills");
    }

    public void Navigate(InputAction.CallbackContext context)
    {
        if (Time.time >= lastNavigationTime + navigationCooldown)
        {
            Vector2 navValue = context.ReadValue<Vector2>();
            if (navValue.y > 0)
            {
                NavigateUp();
            }
            else if (navValue.y < 0)
            {
                NavigateDown();
            }
            else if (navValue.x > 0)
            {
                NavigateRight();
            }
            else if (navValue.x < 0)
            {
                NavigateLeft();
            }
            lastNavigationTime = Time.time;
        }
    }

    private void NavigateLeft()
    {
        int newRow = currentlyHoveredIndex / 6;
        int newColumn = (currentlyHoveredIndex % 6 + 5) % 6;
        UpdateHoveredAbility(newRow * 6 + newColumn);
    }

    private void NavigateRight()
    {
        int newRow = currentlyHoveredIndex / 6;
        int newColumn = (currentlyHoveredIndex % 6 + 1) % 6;
        UpdateHoveredAbility(newRow * 6 + newColumn);
    }

    private void NavigateUp()
    {
        int newRow = (currentlyHoveredIndex / 6 + 1) % 2;
        int newColumn = currentlyHoveredIndex % 6;
        UpdateHoveredAbility(newRow * 6 + newColumn);
    }

    private void NavigateDown()
    {
        int newRow = (currentlyHoveredIndex / 6 + 1) % 2;
        int newColumn = currentlyHoveredIndex % 6;
        UpdateHoveredAbility(newRow * 6 + newColumn);
    }

    private void UpdateHoveredAbility(int newIndex)
    {
        allAbilities[currentlyHoveredIndex].GetComponent<UnusedAbility>().DisableHover();

        currentlyHoveredIndex = newIndex;

        allAbilities[currentlyHoveredIndex].GetComponent<UnusedAbility>().ActivateHover();
    }
}
