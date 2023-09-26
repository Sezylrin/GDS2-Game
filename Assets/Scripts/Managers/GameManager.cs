using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem;
using KevinCastejon.MoreAttributes;
public enum ControlScheme
{
    keyboardAndMouse,
    controller
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SceneLoader sceneLoader { get; private set; }
    [field: SerializeField] public EnemyManager EnemyManager { get; private set; }
    [field: SerializeField] public TimerManager TimerManager { get; private set; }
    [field: SerializeField] public PoolingManager PoolingManager { get; private set; }
    [field: SerializeField] public ElementCombo ComboManager { get; private set; }
    [field: SerializeField] public LevelGenerator LevelGenerator { get; private set; }
    [field: SerializeField] public SkillTreeManager SkillTreeManager { get; private set; }
    [field: SerializeField] public SkillSwitchManager SkillSwitchManager { get; private set; }
    [field: SerializeField] public StatsManager StatsManager { get; private set; }
    public int Souls { get; private set; }
    [field: SerializeField, HideOnPlay(true)]
    public Transform PlayerTransform { get; private set; }
    public PlayerComponentManager PCM { get; private set; }
    private InteractionBase interaction;
    private Consume consume;
    #region ControlScheme
    [field: SerializeField]
    public ControlScheme currentScheme { get; private set; }
    public InputUser User { get; private set; }
    public EventHandler OnControlSchemeSwitch;
    #endregion

    #region Cursor
    [field: SerializeField]
    public Transform controllerCursosrTR { get;private set; }
    [field: SerializeField]
    public SpriteRenderer controllerCursorRend { get; private set; }
    #endregion

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InputUser.onChange += SetScheme;
        SwitchToMouseCursor();
    }


    public void SetPlayerTransform(Transform player, PlayerComponentManager PCM)
    {
        PlayerTransform = player;
        this.PCM = PCM;
    }
    #region Souls
    public void AddSouls(int souls)
    {
        Souls += souls;
        PCM.UI.UpdateSoulsText();
    }

    public void RemoveSouls(int souls)
    {
        Souls -= souls;
        PCM.UI.UpdateSoulsText();
    }

    public void SetSoulsToZero()
    {
        Souls = 0;
    }
    #endregion

    #region Interaction
    public void SetInteraction(InteractionBase interaction)
    {
        this.interaction = interaction;
    }

    public void RemoveInteraction(InteractionBase interaction)
    {
        if (this.interaction == interaction)
        {
            this.interaction = null;
        }
    }

    public void CallInteraction()
    {
        if (interaction)
        {
            interaction.Interact();
        }
    }
    #endregion

    #region Consume (healing)
    public void SetConsume(Consume consume)
    {
        this.consume = consume;
    }

    public void RemoveConsume(Consume consume)
    {
        if (this.consume == consume)
        {
            this.consume = null;
        }
    }

    public void CallConsume()
    {
        if (consume)
        {
            consume.TriggerConsume();
        }
    }
    #endregion

    #region ControlScheme
    public void SetScheme(InputUser inputUser, InputUserChange change, InputDevice device)
    {
        if (!change.Equals(InputUserChange.ControlSchemeChanged))
            return;
        if (inputUser.controlScheme.Equals(null))
            return;
        InputControlScheme temp = (InputControlScheme)inputUser.controlScheme;
        ControlScheme scheme;
        switch (temp.name)
        {
            case "Keyboard&Mouse":
                scheme = ControlScheme.keyboardAndMouse;
                SwitchToMouseCursor();
                break;
            case "Controller":
                scheme = ControlScheme.controller;
                SwitchToControllerCursor();
                break;
            default:
                scheme = ControlScheme.keyboardAndMouse;
                SwitchToMouseCursor();
                break;
        }
        User = inputUser;
        currentScheme = scheme;
        OnControlSchemeSwitch?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Cursor
    public void SwitchToMouseCursor()
    {
        Cursor.visible = true;
        controllerCursosrTR.gameObject.SetActive(false);
    }
    public void SwitchToControllerCursor()
    {
        Cursor.visible = false;
        controllerCursosrTR.gameObject.SetActive(true);
    }
    public void HideControllerCursor()
    {
        if(controllerCursorRend.enabled)
            controllerCursorRend.enabled = false;
    }

    public void ShowControllerCursor()
    {
        if (!controllerCursorRend.enabled)
            controllerCursorRend.enabled = true;
    }
    #endregion
}
