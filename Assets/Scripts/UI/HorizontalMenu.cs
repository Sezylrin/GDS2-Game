using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalMenu : Menu
{
    [SerializeField]
    BookButton[] buttons;

    private int currentSelectedIndex = 0;

    private float navigationCooldownDuration = 0.15f;
    private bool isOnCooldown = false;

    public override void OpenMenu()
    {
        SetActiveButton(0);
    }

    public void SetActiveButton(int index)
    {
        if (index < 0)
            index = buttons.Length - 1;
        else if (index >= buttons.Length)
            index = 0;

        buttons[currentSelectedIndex].DisableHover();

        currentSelectedIndex = index;

        buttons[currentSelectedIndex].ActivateHover();
    }

    public void ClickActiveButton()
    {
        buttons[currentSelectedIndex].HandleClick();
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        if (!isOnCooldown)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            if (direction.y > 0.5f)
                SetActiveButton(currentSelectedIndex - 1);
            else if (direction.y < -0.5f)
                SetActiveButton(currentSelectedIndex + 1);

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

    private void OnEnable()
    {
        //BookButton.OnButtonHovered += UpdateSelectedIndex;
    }

    private void OnDisable()
    {
        //BookButton.OnButtonHovered -= UpdateSelectedIndex;
    }

    private void UpdateSelectedIndex(BookButton hoveredButton)
    {
        int index = System.Array.IndexOf(buttons, hoveredButton);
        if (index != -1)
        {
            currentSelectedIndex = index;
        }
    }
}
