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
    [SerializeField] private BookButton returnButton;

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

    [Header("Info")]
    [SerializeField] private TMP_Text SoulsText;
    [SerializeField] private TMP_Text HealthText;
    [SerializeField] private TMP_Text DamageText;

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

        abilityButtonRows[0].row[0].UpdatePopup();
    }

    public override void Return()
    {
        ReturnToMainMenu();
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.UIManager.GetBookMenu().ReturnToMainMenu();
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

    public void UpdateHealthText()
    {
        int StartingHealth = 100;
        int BonusHealth = GameManager.Instance.StatsManager.bonusHealth;
        HealthText.text = (StartingHealth + BonusHealth).ToString();
    }

    public void UpdateDamageText()
    {
        float DamageModifier = GameManager.Instance.StatsManager.damageModifier;
        string DamagePercentage = DamageModifier * 100 + "%";
        DamageText.text = DamagePercentage;
    }

    public void SetActiveButton(int row, int column)
    {
        if (isOnAbilityMenu && row == abilityRows)
        {
            // If a valid button was previously selected, disable its hover state
            if (currentSelectedIndex.x != -1 && currentSelectedIndex.y != -1)
            {
                abilityButtonRows[currentSelectedIndex.x].row[currentSelectedIndex.y].DisableHover();
            }
            returnButton.ActivateHover();
            currentSelectedIndex = new Vector2Int(-1, -1); // Use -1,-1 to indicate the return button is selected
            return;
        }

        // If the return button is currently selected and we move up, go to the last ability row
        if (isOnAbilityMenu && currentSelectedIndex == new Vector2Int(-1, -1) && row < abilityRows)
        {
            returnButton.DisableHover();
            row = abilityRows - 1;
            column = 0; // Set to the first column of the last ability row
        }

        List<ButtonRow> departingList = isOnAbilityMenu ? abilityButtonRows : upgradeButtonRows;

        // Handle column boundaries and switch menu if needed
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

        // Handle row boundaries
        if (row < 0)
            row = isOnAbilityMenu ? abilityRows - 1 : upgradeRows - 1;
        else if (isOnAbilityMenu && row >= abilityRows)
            row = 0;
        else if (!isOnAbilityMenu && row >= upgradeRows)
            row = 0;

        // Disable hover for the departing menu button
        if (currentSelectedIndex.x != -1 && currentSelectedIndex.y != -1) // Make sure we are not departing from the return button
        {
            departingList[currentSelectedIndex.x].row[currentSelectedIndex.y].DisableHover();
        }

        currentSelectedIndex = new Vector2Int(row, column);

        List<ButtonRow> arrivingList = isOnAbilityMenu ? abilityButtonRows : upgradeButtonRows;

        // Enable hover for arriving menu button
        arrivingList[currentSelectedIndex.x].row[currentSelectedIndex.y].ActivateHover();
    }

    public void ClickActiveButton()
    {
        if (currentSelectedIndex == new Vector2Int(-1, -1))
        {
            returnButton.HandleClick();
            return;
        }

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