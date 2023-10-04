using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

enum CurrentSkillMenu
{
    UnusedAbilities,
    ActiveAbilities,
}

public class SkillSwitchManager : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField]
    private GameObject allAbilitiesContainer;
    [SerializeField]
    private GameObject activeAbilitiesContainer;
    [SerializeField]
    private GameObject popupContainer;

    [Header("Popup")]
    [SerializeField]
    private TMP_Text abilityName;
    [SerializeField]
    private TMP_Text abilityDescription;
    [SerializeField]
    private GameObject notYetUnlocked;

    private List<GameObject> allAbilities = new();
    private List<GameObject> activeAbilities = new();

    private UIAbility currentlyHoveredAbility;
    private UIAbility selectedAbility;
    private int currentlyHoveredIndex = 0;

    private float navigationCooldown = 0.15f; 
    private float lastNavigationTime;
    private CurrentSkillMenu currentSkillMenu = CurrentSkillMenu.UnusedAbilities;
    private bool firstSkillsetSelected = true;

    private AudioComponent audioComponent;

    private void Awake()
    {
        InitialiseSkillSwitchManager();
    }

    private void Start()
    {
        audioComponent = GetComponent<AudioComponent>();
    }

    public void InitialiseSkillSwitchManager()
    {
        StatsManager statsManager = GameManager.Instance.StatsManager;
        List<ElementalSO> unlockedAbilities = statsManager.GetUnlockedAbilities();
        List<ElementalSO> lockedAbilities = statsManager.GetLockedAbilities();

        int abilityIndex = 0;

        //Initialise All Abilities
        for (int i = 0; i < allAbilitiesContainer.transform.childCount; i++)
        {
            Transform childTransform = allAbilitiesContainer.transform.GetChild(i);
            UnusedAbility unusedAbility = childTransform.gameObject.GetComponent<UnusedAbility>();
            if (unusedAbility != null)
            {
                if (abilityIndex < unlockedAbilities.Count)
                {
                    unusedAbility.abilityData = unlockedAbilities[abilityIndex];
                    unusedAbility.UpdateBorder();
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
            Transform childTransform = activeAbilitiesContainer.transform.GetChild(i).transform.GetChild(0);
            activeAbilities.Add(childTransform.gameObject);
        }

        UnusedAbility firstAbility = allAbilities[0].GetComponent<UnusedAbility>();
        firstAbility.ActivateHover(false);
        currentlyHoveredAbility = firstAbility;

        //Initialise Active Abilities
        Dictionary<int, ElementalSO> savedAbilityPositions = statsManager.savedAbilityPositions;
        for (int i = 0; i < activeAbilitiesContainer.transform.childCount; i++)
        {
            Transform childTransform = activeAbilitiesContainer.transform.GetChild(i).transform.GetChild(0);
            ActiveAbility activeAbility = childTransform.GetComponent<ActiveAbility>();
            if (activeAbility != null && savedAbilityPositions.ContainsKey(i))
            {
                activeAbility.UpdateAbility(savedAbilityPositions[i]);
                childTransform.GetChild(0).GetComponent<Image>().sprite = activeAbility.abilityData.icon;
            }
            else
            {
                childTransform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }

    #region Interactions
    public void OpenMenu()
    {
        if (gameObject.activeSelf)
            return;
        gameObject.SetActive(true);
        GameManager.Instance.AudioComponent.PlaySound(SoundType.UIOpenMenu);
        currentlyHoveredIndex = 0;
        currentSkillMenu = CurrentSkillMenu.UnusedAbilities;
        firstSkillsetSelected = true;
        InitialiseSkillSwitchManager();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        StatsManager statsManager = GameManager.Instance.StatsManager;
        if (currentSkillMenu == CurrentSkillMenu.UnusedAbilities)
        {
            if (!statsManager.GetUnlockedAbilities().Contains(currentlyHoveredAbility.abilityData))
            {
                audioComponent.PlaySound(SoundType.UIClickFail);
                return;
            }
            currentSkillMenu = CurrentSkillMenu.ActiveAbilities;
            currentlyHoveredAbility.DisableHover();
            selectedAbility = currentlyHoveredAbility;
            UIAbility newSelectedAbility = activeAbilitiesContainer.transform.GetChild(0).transform.GetChild(0).GetComponent<UIAbility>();
            currentlyHoveredAbility = newSelectedAbility;
            currentlyHoveredIndex = 0;
            newSelectedAbility.ActivateHover();
        }
        else
        {
            int offset = firstSkillsetSelected ? 0 : 3;
            int hoveredIndex = currentlyHoveredIndex + offset;
            statsManager.savedAbilityPositions[hoveredIndex] = selectedAbility.abilityData;
            GameManager.Instance.PCM.abilities.SetSlot(selectedAbility.abilityData, hoveredIndex);
            currentSkillMenu = CurrentSkillMenu.UnusedAbilities;
            currentlyHoveredAbility.DisableHover();
            ActiveAbility ability = (ActiveAbility)currentlyHoveredAbility;
            ability.UpdateAbility(selectedAbility.abilityData);
            UIAbility newSelectedAbility = allAbilitiesContainer.transform.GetChild(0).GetComponent<UIAbility>();
            currentlyHoveredAbility = newSelectedAbility;
            currentlyHoveredIndex = 0;
            audioComponent.PlaySound(SoundType.UIClick);
            newSelectedAbility.ActivateHover(false);
            selectedAbility = null;
        }
    }

    public void ToggleSkills(InputAction.CallbackContext context)
    {
        StatsManager statsManager = GameManager.Instance.StatsManager;
        if (currentSkillMenu == CurrentSkillMenu.ActiveAbilities && statsManager.secondSkillsetUnlocked)
        {
            firstSkillsetSelected = !firstSkillsetSelected;

            Dictionary<int, ElementalSO> savedAbilityPositions = statsManager.savedAbilityPositions;

            int offset = firstSkillsetSelected ? 0 : 3;

            for (int i = 0; i < activeAbilitiesContainer.transform.childCount; i++)
            {
                int dictionaryKey = i + offset;
                Transform childTransform = activeAbilitiesContainer.transform.GetChild(i).transform.GetChild(0);
                ActiveAbility activeAbility = childTransform.GetComponent<ActiveAbility>();

                if (savedAbilityPositions.ContainsKey(dictionaryKey))
                {
                    activeAbility.UpdateAbility(savedAbilityPositions[dictionaryKey]);
                }
                else
                {
                    activeAbility.abilityData = null;
                    childTransform.GetChild(0).GetComponent<Image>().enabled = false;
                    activeAbility.GreyBorder(currentlyHoveredIndex == i);
                }

                if (currentlyHoveredIndex == i)
                {
                    activeAbility.ActivateHover();
                }
            }
        }
    }


    public void Return(InputAction.CallbackContext context)
    {
        if (currentSkillMenu == CurrentSkillMenu.UnusedAbilities)
        {
            GameManager.Instance.AudioComponent.PlaySound(SoundType.UICloseMenu);
            gameObject.SetActive(false);
            GameManager.Instance.PlayerTransform.gameObject.SetActive(true);
        }
        else
        {
            currentSkillMenu = CurrentSkillMenu.UnusedAbilities;
            currentlyHoveredAbility.DisableHover();
            UIAbility newSelectedAbility = allAbilitiesContainer.transform.GetChild(0).GetComponent<UIAbility>();
            currentlyHoveredAbility = newSelectedAbility;
            currentlyHoveredIndex = 0;
            newSelectedAbility.ActivateHover();
            selectedAbility = null;
        }
    }
    #endregion

    public void UpdatePopup(ElementalSO abilityData)
    {
        if (!abilityData)
        {
            popupContainer.SetActive(false);
            return;
        }
        popupContainer.SetActive(true);
        abilityName.text = abilityData.name;
        string s = "";
        if (abilityData.castCost != 1) s = "s";
        abilityDescription.text = abilityData.description + ". Costs " + abilityData.castCost + " point" + s + " to cast.";
        if (!GameManager.Instance.StatsManager.GetUnlockedAbilities().Contains(abilityData))
        {
            notYetUnlocked.SetActive(true);
        }
        else
        {
            notYetUnlocked.SetActive(false);
        }
    }

    #region Navigate
    public void Navigate(InputAction.CallbackContext context)
    {
        if (Time.time >= lastNavigationTime + navigationCooldown)
        {
            Vector2 navValue = context.ReadValue<Vector2>();
            if (currentSkillMenu == CurrentSkillMenu.UnusedAbilities)
            {
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
            }
            else
            {
                if (navValue.x > 0)
                {
                    NavigateRightActive();
                }
                else if (navValue.x < 0)
                {
                    NavigateLeftActive();
                }
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

    private void NavigateLeftActive()
    {
        int newIndex = (currentlyHoveredIndex + 2) % 3;
        UpdateActiveHoveredAbility(newIndex);
    }

    private void NavigateRightActive()
    {
        int newIndex = (currentlyHoveredIndex + 1) % 3;
        UpdateActiveHoveredAbility(newIndex);
    }

    private void UpdateHoveredAbility(int newIndex)
    {
        allAbilities[currentlyHoveredIndex].GetComponent<UIAbility>().DisableHover();

        currentlyHoveredIndex = newIndex;
        UIAbility newAbility = allAbilities[currentlyHoveredIndex].GetComponent<UIAbility>();
        currentlyHoveredAbility = newAbility;
        newAbility.ActivateHover();
    }

    private void UpdateActiveHoveredAbility(int newIndex)
    {
        activeAbilities[currentlyHoveredIndex].GetComponent<UIAbility>().DisableHover();

        currentlyHoveredIndex = newIndex;
        UIAbility newAbility = activeAbilities[currentlyHoveredIndex].GetComponent<UIAbility>();
        currentlyHoveredAbility = newAbility;
        newAbility.ActivateHover();
    }
    #endregion
}
