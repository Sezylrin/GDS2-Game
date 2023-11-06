using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using KevinCastejon.MoreAttributes;
using UnityEngine.InputSystem.Utilities;
public enum ControlScheme
{
    keyboardAndMouse,
    controller
}
public enum ControllerScheme
{
    Playstation,
    Xbox,
    Switch
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
    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public MusicManager MusicManager { get; private set; }
    [field: SerializeField] public UIManager UIManager { get; private set; }
   
    [field: SerializeField, HideOnPlay(true)] public Transform PlayerTransform { get; private set; }
    [field: SerializeField] public StatsManager StatsManager { get; private set; }
    [field: SerializeField] private GameObject BookMenuObj;
    [field: SerializeField] public TileSwapper TileSwapper { get; private set; }
    [field: SerializeField, HideOnPlay(true)]
    public PlayerComponentManager PCM { get; private set; }
    public Transform CameraTrackPoint { get; private set; }
    private InteractionBase interaction;

    public PlayerInputs playerInputs { get; private set; }

    private Consume consume;
    [field: SerializeField] public int HealthPerSegment { get; set; } = 100;

    public bool IsTutorial { get; private set; }
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
        }
        playerInputs = new PlayerInputs();
        konami = playerInputs.Konami;
        cheat = playerInputs.Cheat;
        input.onControlsChanged += SetScheme;
        
        SwitchToMouseCursor();
    }

    private void Start()
    {
        ConsumeTimers = TimerManager.GenerateTimers(typeof(ConsumeTimer), gameObject);
        ConsumeTimers.times[(int)ConsumeTimer.consumeDelay].OnTimeIsZero += EndConsumeDelay;
        if (!PlayerTransform)
        {
            GameObject temp = GameObject.FindWithTag(Tags.T_Player);
            if (temp)
            {
                PCM = temp.GetComponent<PlayerComponentManager>();
                PlayerTransform = temp.transform;
            }

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
            RemoveInteraction(interaction);
        }
    }
    #endregion

    #region Consume (healing)
    protected enum ConsumeTimer
    {
        consumeDelay
    }

    [field: SerializeField] private float ConsumeDelayDuration { get; set; } = 1;
    [field: SerializeField] private Timer ConsumeTimers { get; set; }
    private bool consuming = false;

    public void SetConsume(Consume consume)
    {
        this.consume = consume;
    }

    public void RemoveConsume(Consume consume)
    {
        if (this.consume == consume && !consuming)
        {
            this.consume = null;
        }
    }

    public void CallConsume()
    {
        if (consume)
        {
            StartConsumeDelay();
            consuming = true;
        }
    }

    private void StartConsumeDelay()
    {
        ConsumeTimers.SetTime((int)ConsumeTimer.consumeDelay, ConsumeDelayDuration);
    }

    private void EndConsumeDelay(object sender, EventArgs e)
    {
        if (consume) RemoveConsume(consume);
    }

    #endregion

    #region ControlScheme
    [field: SerializeField]
    public ControlScheme currentScheme { get; private set; }

    [field: SerializeField]

    public PlayerInput input { get; private set; }

    public EventHandler OnControlSchemeSwitch;
    public ControllerScheme controllerType;
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
                var Pad = Gamepad.current;
                if (Pad is XInputController)
                    controllerType = ControllerScheme.Xbox;
                else if (Pad is DualShockGamepad)
                    controllerType = ControllerScheme.Playstation;
                else if (Pad is SwitchProControllerHID)
                    controllerType = ControllerScheme.Switch;

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

    #region Setter
    public void SetIsTutorial(bool tutorial)
    {
        IsTutorial = tutorial;
        StatsManager.ResetEquipForTutorial();
    }
    #endregion

    #region Konami
    public bool cheatMode { get; private set; } = false;
    private PlayerInputs.KonamiActions konami;
    private PlayerInputs.CheatActions cheat;
    private int codePosition = 0;
    private KoIn[] konamiCode = { KoIn.up, KoIn.up, KoIn.down, KoIn.down, KoIn.left, KoIn.right, KoIn.left, KoIn.right, KoIn.B, KoIn.A, KoIn.Return };
    private enum KoIn
    {
        left,
        right,
        up,
        down,
        A,
        B,
        Return
    }

    private void OnEnable()
    {
        
        konami.Enable();
        konami.Left.performed += Left;
        konami.Right.performed += Right;
        konami.Up.performed += Up;
        konami.Down.performed += Down;
        konami.A.performed += A;
        konami.B.performed += B;
        konami.Return.performed += Return;
    }
    private void DisableKonami()
    {
        konami.Left.performed -= Left;
        konami.Right.performed -= Right;
        konami.Up.performed -= Up;
        konami.Down.performed -= Down;
        konami.A.performed -= A;
        konami.B.performed -= B;
        konami.Return.performed -= Return;
        konami.Disable();
    }
    private void CheckCode(KoIn input)
    {
        if (konamiCode[codePosition] == input)
        {
            codePosition++;
            if(input == KoIn.Return)
            {
                AudioManager.PlaySound(AudioRef.Victory);
                cheatMode = true;
                DisableKonami();
                cheat.Enable();
                cheat.Kill.performed += KillEnemy;
                cheat.Souls.performed += GainSouls;
            }
        }
        else
        {
            codePosition = 0;
        }
    }
    private void KillEnemy(InputAction.CallbackContext context)
    {
        EnemyManager.KillEnemies();
    }private void GainSouls(InputAction.CallbackContext context)
    {
        AddSouls(10000);
    }
    private void Left(InputAction.CallbackContext context)
    {
        CheckCode(KoIn.left);
    }
    private void Right(InputAction.CallbackContext context)
    {
        CheckCode(KoIn.right);
    }
    private void Up(InputAction.CallbackContext context)
    {
        CheckCode(KoIn.up);
    }
    private void Down(InputAction.CallbackContext context)
    {
        CheckCode(KoIn.down);
    }
    private void A(InputAction.CallbackContext context)
    {
        CheckCode(KoIn.A);
    }
    private void B(InputAction.CallbackContext context)
    {
        CheckCode(KoIn.B);
    }
    private void Return(InputAction.CallbackContext context)
    {
        CheckCode(KoIn.Return);
    }
    #endregion
}