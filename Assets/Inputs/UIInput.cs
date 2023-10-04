using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInput : MonoBehaviour
{
    private PlayerInputs input;
    private PlayerInputs.UIActions ui;

    [SerializeField] 
    private SkillSwitchManager skillSwitchManager;

    private void Awake()
    {
        input = GameManager.Instance.playerInputs;
        ui = input.UI;
    }


    private void OnEnable()
    {
        ui.Enable();
        ui.ToggleSkill.performed += skillSwitchManager.ToggleSkills;
        ui.Interact.performed += skillSwitchManager.Interact;
        ui.Navigate.performed += skillSwitchManager.Navigate;
        ui.Return.performed += skillSwitchManager.Return;
    }

    private void OnDisable()
    {
        ui.ToggleSkill.performed -= skillSwitchManager.ToggleSkills;
        ui.Interact.performed -= skillSwitchManager.Interact;
        ui.Navigate.performed -= skillSwitchManager.Navigate;
        ui.Return.performed -= skillSwitchManager.Return;
        ui.Disable();
    }
}
