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
    private PlayerController playerController;
    private void Awake()
    {
        input = new PlayerInput();
        player = input.Player;
    }

    private void OnEnable()
    {
        player.Enable();
        player.Move.performed += playerController.SetDirection;
        player.Move.canceled += playerController.SetDirection;
        player.Attack.performed += playerController.BufferLightAttack;
        player.Dash.performed += playerController.BufferDash;
    }

    private void OnDisable()
    {
        player.Move.performed -= playerController.SetDirection;
        player.Move.canceled -= playerController.SetDirection;
        player.Attack.performed -= playerController.BufferLightAttack;
        player.Dash.performed -= playerController.BufferDash;
        player.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
