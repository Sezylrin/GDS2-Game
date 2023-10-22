using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInput : MonoBehaviour
{
    private PlayerInputs input;
    private PlayerInputs.UIActions ui;

    [SerializeField] 
    private BookMenu bookMenu;

    private void Awake()
    {
        input = GameManager.Instance.playerInputs;
        ui = input.UI;
    }

    private void OnEnable()
    {
        ui.Enable();
        ui.ToggleSkill.performed += bookMenu.ToggleSkills;
        ui.Interact.performed += bookMenu.Interact;
        ui.Navigate.performed += bookMenu.Navigate;
        ui.Return.performed += bookMenu.Return;
        ui.Escape.performed += bookMenu.ToggleOpenMenu;
    }

    private void OnDisable()
    {
        ui.ToggleSkill.performed -= bookMenu.ToggleSkills;
        ui.Interact.performed -= bookMenu.Interact;
        ui.Navigate.performed -= bookMenu.Navigate;
        ui.Return.performed -= bookMenu.Return;
        ui.Escape.performed -= bookMenu.ToggleOpenMenu;
        ui.Disable();
    }
}
