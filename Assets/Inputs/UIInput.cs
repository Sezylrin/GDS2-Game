using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInput : MonoBehaviour
{
    private PlayerInputs input;
    private PlayerInputs.UIActions ui;

    [SerializeField] 
    private UIManager UIManager;

    private void Awake()
    {
        input = GameManager.Instance.playerInputs;
        ui = input.UI;
    }

    private void OnEnable()
    {
        ui.Enable();
        ui.ToggleSkill.performed += UIManager.ToggleSkills;
        ui.Interact.performed += UIManager.Interact;
        ui.Navigate.performed += UIManager.Navigate;
        ui.Return.performed += UIManager.Return;
        ui.Escape.performed += UIManager.ToggleOpenMenu;
    }

    private void OnDisable()
    {
        ui.ToggleSkill.performed -= UIManager.ToggleSkills;
        ui.Interact.performed -= UIManager.Interact;
        ui.Navigate.performed -= UIManager.Navigate;
        ui.Return.performed -= UIManager.Return;
        ui.Escape.performed -= UIManager.ToggleOpenMenu;
        ui.Disable();
    }
}
