using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerInput input;
    private PlayerInput.PlayerActions player;

    [SerializeField]
    private PlayerComponentManager PCM;
    private void Awake()
    {
        input = new PlayerInput();
        player = input.Player;
    }

    private void OnEnable()
    {
        player.Enable();
        player.Move.performed += PCM.control.SetDirection;
        player.Move.canceled += PCM.control.SetDirection;
        player.Attack.performed += PCM.control.BufferLightAttack;
        player.Dash.performed += PCM.control.BufferDash;
        player.AbilityOne.performed += PCM.control.BufferAbilityOne;
        player.AbilityTwo.performed += PCM.control.BufferAbilityTwo;
        player.AbilityThree.performed += PCM.control.BufferAbilityThree;
        player.ToggleAbilities.performed += PCM.abilities.ToggleActiveAbilitySet;

        GameManager.Instance.SetPlayerTransform(transform, PCM);
    }

    private void OnDisable()
    {
        GameManager.Instance.SetPlayerTransform(null, null);

        player.Move.performed -= PCM.control.SetDirection;
        player.Move.canceled -= PCM.control.SetDirection;
        player.Attack.performed -= PCM.control.BufferLightAttack;
        player.Dash.performed -= PCM.control.BufferDash;
        player.AbilityOne.performed -= PCM.control.BufferAbilityOne;
        player.AbilityTwo.performed -= PCM.control.BufferAbilityTwo;
        player.AbilityThree.performed -= PCM.control.BufferAbilityThree;
        player.ToggleAbilities.performed -= PCM.abilities.ToggleActiveAbilitySet;
        player.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
