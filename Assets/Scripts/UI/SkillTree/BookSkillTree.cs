using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ButtonRow
{
    public List<BaseSkillTreeButton> row = new List<BaseSkillTreeButton>();
}

public class BookSkillTree : Menu
{
    [Header("Menus")]
    [SerializeField] private List<ButtonRow> abilityButtonRows;
    [SerializeField] private List<ButtonRow> upgradeButtonRows;

    [Header("Dimensions")]
    [SerializeField] private int abilityRows = 2;
    [SerializeField] private int abilityColumns = 7;
    [SerializeField] private int upgradeRows = 1;
    [SerializeField] private int upgradeColumns = 2;

    [Header("Popup")]
    [SerializeField] private TMP_Text popupName;
    [SerializeField] private TMP_Text popupDescription;
    [SerializeField] private GameObject cantAfford;
    [SerializeField] private GameObject purchased;

    [Header("Souls")]
    [SerializeField] private TMP_Text SoulsText;

    private Vector2Int currentSelectedIndex = new Vector2Int(0, 0);
    private bool isOnAbilityMenu = true;

    private float navigationCooldownDuration = 0.15f;
    private bool isOnCooldown = false;

    public override void OpenMenu()
    {
        isOnAbilityMenu = true;
        SetActiveButton(0, 0);
        SoulsText.text = GameManager.Instance.Souls.ToString();

        foreach (ButtonRow row in abilityButtonRows)
        {
            foreach (BaseSkillTreeButton button in row.row)
            {
                button.Init();
            }
        }
        foreach (ButtonRow row in upgradeButtonRows)
        {
            foreach (BaseSkillTreeButton button in row.row)
            {
                button.Init();
            }
        }
    }

    public void UpdatePopup(string name, string description, bool canAfford, bool showPurchased)
    {
        popupName.text = name;
        popupDescription.text = description;
        if (!canAfford && !showPurchased)
        {
            purchased.SetActive(false);
            cantAfford.SetActive(true);
            return;
        }
        if (showPurchased)
        {
            cantAfford.SetActive(false);
            purchased.SetActive(true);
            return;
        }
        purchased.SetActive(false);
        cantAfford.SetActive(false);
    }

    public void UpdateSoulsText(int souls)
    {
        SoulsText.text = souls.ToString();
    }

    public void SetActiveButton(int row, int column)
    {
        List<ButtonRow> departingList = isOnAbilityMenu ? abilityButtonRows : upgradeButtonRows;

        // Check boundary conditions and switch menu if needed
        if (column < 0)
        {
            if (isOnAbilityMenu)
            {
                column = abilityColumns - 1; // loop around within the ability menu
            }
            else
            {
                isOnAbilityMenu = true;
                column = abilityColumns - 1; // switch to the last column of the ability menu
            }
        }
        else if (column >= (isOnAbilityMenu ? abilityColumns : upgradeColumns))
        {
            if (isOnAbilityMenu)
            {
                isOnAbilityMenu = false;
                column = 0; // switch to the first column of the upgrade menu
            }
            else
            {
                column = 0; // loop around within the upgrade menu
            }
        }

        if (row < 0)
            row = isOnAbilityMenu ? abilityRows - 1 : upgradeRows - 1;
        else if (isOnAbilityMenu && row >= abilityRows)
            row = 0;
        else if (!isOnAbilityMenu && row >= upgradeRows)
            row = 0;

        // Disable hover for departing menu button
        departingList[currentSelectedIndex.x].row[currentSelectedIndex.y].DisableHover();

        currentSelectedIndex = new Vector2Int(row, column);

        List<ButtonRow> arrivingList = isOnAbilityMenu ? abilityButtonRows : upgradeButtonRows;

        // Enable hover for arriving menu button
        arrivingList[currentSelectedIndex.x].row[currentSelectedIndex.y].ActivateHover();
    }

    public void ClickActiveButton()
    {
        List<ButtonRow> currentList = isOnAbilityMenu ? abilityButtonRows : upgradeButtonRows;
        currentList[currentSelectedIndex.x].row[currentSelectedIndex.y].HandlePurchase();
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        if (!isOnCooldown)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            if (direction.y > 0.5f)
                SetActiveButton(currentSelectedIndex.x - 1, currentSelectedIndex.y);
            else if (direction.y < -0.5f)
                SetActiveButton(currentSelectedIndex.x + 1, currentSelectedIndex.y);
            else if (direction.x > 0.5f)
                SetActiveButton(currentSelectedIndex.x, currentSelectedIndex.y + 1);
            else if (direction.x < -0.5f)
                SetActiveButton(currentSelectedIndex.x, currentSelectedIndex.y - 1);

            StartCoroutine(NavigationCooldown());
        }
    }

    public override void Interact()
    {
        ClickActiveButton();
    }

    private IEnumerator NavigationCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSecondsRealtime(navigationCooldownDuration);
        isOnCooldown = false;
    }
}