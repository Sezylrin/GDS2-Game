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

    [field: SerializeField] public SceneLoader sceneLoader { get; private set; }
    [field: SerializeField] public EnemyManager EnemyManager { get; private set; }
    [field: SerializeField] public TimerManager TimerManager { get; private set; }
    [field: SerializeField] public PoolingManager PoolingManager { get; private set; }
    [field: SerializeField] public ElementCombo ComboManager { get; private set; }
    [field: SerializeField] public LevelGenerator LevelGenerator { get; private set; }
    [field: SerializeField] public SkillTreeManager SkillTreeManager { get; private set; }
    [field: SerializeField] public StatsManager StatsManager { get; private set; }
    [field: SerializeField, HideOnPlay(true)] public Transform PlayerTransform { get; private set; }
    public PlayerComponentManager PCM { get; private set; }
    public Transform CameraTrackPoint { get; private set; }
    private InteractionBase interaction;
    private Consume consume;

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
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            input.onControlsChanged += SetScheme;
            SwitchToMouseCursor();
        }
    }


    public void SetPlayerTransform(Transform player, PlayerComponentManager PCM)
    {
        PlayerTransform = player;
        this.PCM = PCM;
    }
    #region Souls
    public int Souls { get; private set; }
    [field: SerializeField]
    public int LostSouls { get; private set; }
    [field:SerializeField]
    public int FountainRoomToSpawn { get; private set; }
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
    public void RemoveLostSouls()
    {
        LostSouls = 0;
        FountainRoomToSpawn = -1;
    }
    public void SetLostSouls()
    {
        LostSouls = Souls;
        FountainRoomToSpawn = LevelGenerator.lastFloorOnExit;
        if (FountainRoomToSpawn == 0)
            FountainRoomToSpawn = 1;
    }

    public void SetSoulsToZero()
    {
        Souls = 0;
        PCM.UI.UpdateSoulsText();
    }

    public void RetrieveSouls()
    {
        AddSouls(LostSouls);
        LostSouls = 0;
        FountainRoomToSpawn = -1;
    }

    public bool IsSoulRetrieveRoom()
    {
        return FountainRoomToSpawn == LevelGenerator.floorsCleared;
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
    [field: SerializeField]
    public ControlScheme currentScheme { get; private set; }
    [field: SerializeField]
    public PlayerInput input { get; private set; }
    public EventHandler OnControlSchemeSwitch;
    public void SetScheme(PlayerInput input)
    {
        if (input.currentControlScheme.Equals(null))
            return;
        string temp = input.currentControlScheme;
        ControlScheme scheme;
        switch (temp)
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

    #region Camera
    public void SetCameraTrack(Transform trackTarget)
    {
        CameraTrackPoint = trackTarget;
    }
    #endregion
}
