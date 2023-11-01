using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Controls;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerInputs input;
    private PlayerInputs.PlayerActions player;
    

    [SerializeField]
    private PlayerComponentManager PCM;
    private void Awake()
    {
        input = GameManager.Instance.playerInputs;
        player = input.Player;
    }

    private void OnEnable()
    {
        if (PlayerComponentManager.Instance != gameObject)
            return;
        //Debug.Log("called");
        player.Enable();
        player.Move.performed += PCM.control.SetDirection;
        player.Move.canceled += PCM.control.SetDirection;
        player.Dash.performed += PCM.control.BufferDash;
        player.AbilityOne.canceled += PCM.control.BufferAbilityOne;
        player.AbilityOne.performed += PCM.control.BufferAbilityOne;
        player.AbilityTwo.canceled += PCM.control.BufferAbilityTwo;
        player.AbilityTwo.performed += PCM.control.BufferAbilityTwo;
        player.AbilityThree.canceled += PCM.control.BufferAbilityThree;
        player.AbilityThree.performed += PCM.control.BufferAbilityThree;
        player.AbilityFour.canceled += PCM.control.BufferAbilityFour;
        player.AbilityFour.performed += PCM.control.BufferAbilityFour;
        player.ToggleAbilities.performed += PCM.abilities.ToggleActiveAbilitySet;
        player.Interact.performed += PCM.control.Interact;
        player.Consume.performed += PCM.control.Consume;
        player.Look.performed += PCM.control.MousePosition;
        player.Look.canceled += PCM.control.MousePosition;
        player.LookMouse.performed += PCM.control.MousePosition;
        PCM.control.SetControllerCursor(GameManager.Instance.controllerCursosrTR);
    }

    

    private void OnDisable()
    {
        if (PlayerComponentManager.Instance != gameObject)
            return;
        //GameManager.Instance.SetPlayerTransform(null, null);
        PCM.control.SetControllerCursor(null);
        player.Move.performed -= PCM.control.SetDirection;
        player.Move.canceled -= PCM.control.SetDirection;
        player.Dash.performed -= PCM.control.BufferDash;
        player.AbilityOne.canceled -= PCM.control.BufferAbilityOne;
        player.AbilityOne.performed -= PCM.control.BufferAbilityOne;
        player.AbilityTwo.canceled -= PCM.control.BufferAbilityTwo;
        player.AbilityTwo.performed -= PCM.control.BufferAbilityTwo;
        player.AbilityThree.canceled -= PCM.control.BufferAbilityThree;
        player.AbilityThree.performed -= PCM.control.BufferAbilityThree;
        player.AbilityFour.canceled -= PCM.control.BufferAbilityFour;
        player.AbilityFour.performed -= PCM.control.BufferAbilityFour;
        player.ToggleAbilities.performed -= PCM.abilities.ToggleActiveAbilitySet;
        player.Interact.performed -= PCM.control.Interact;
        player.Consume.performed -= PCM.control.Consume;
        player.Look.performed -= PCM.control.MousePosition;
        player.Look.canceled -= PCM.control.MousePosition;
        player.LookMouse.performed -= PCM.control.MousePosition;
        player.Disable();
    }
    // Update is called once per frame

    void Update()
    {
        
    }
}
