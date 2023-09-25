using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using KevinCastejon.MoreAttributes;

public enum actionState
{
    nothing,
    dashing,
    attack,
    abilityOne,
    abilityTwo,
    abilityThree
}
public enum playerState
{
    idle,
    moving,
    dashing,
    attack,
    attackEnd,
    abilityCast,
    abilityLag
}
public class PlayerController : MonoBehaviour
{
    private enum coolDownTimers : int
    {
        dashCD,
        abilityCast,
        abilityLag
    }

    [field: Header("Core variables")]
    [field: SerializeField]
    public Rigidbody2D rb { get; private set; }
    [SerializeField]
    private CapsuleCollider2D col2D;
    [SerializeField]
    private PlayerComponentManager PCM;
    
    [Header("Speed Stats")]

    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float maxSpeed;
    [SerializeField] [ReadOnly]
    private float currentMaxSpeed;

    [Header("Dash Stats")]

    [SerializeField]
    private float dashCDTimer;
    [SerializeField]
    private float dashDistance;
    [SerializeField]
    private float dashDuration;
    [SerializeField]
    private int dashCharges;
    [SerializeField]
    private float dashRechargeRate;
    [SerializeField] [ReadOnly]
    private float currentDashCharges;

    [field: Header("Ability")]
    [SerializeField]
    private float abilityLag;
    [SerializeField]
    private float abilityCast;

    [field:Header("Transforms")]
    [field:SerializeField]
    public Transform AbilityCentre { get; private set; } 

    [Header("Input Buffer")]

    [SerializeField]
    private int bufferDuration;
    private int currentBufferDuration;

    [Header("Others")]

    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private Transform cursorPos;

    [Header("Debug Values")]

    [SerializeField] [ReadOnly]
    private Vector2 direction;
    [SerializeField] [ReadOnly]
    private Vector2 LastDirection;
    [field: SerializeField] [field: ReadOnly]
    public playerState CurrentState { get; private set; }
    [SerializeField] [ReadOnly]
    private actionState bufferedState;
    public Vector2 mousePos { get; private set; }

    private float drag;


    private Coroutine dashCoroutine;
    private Timer timers;
    [SerializeField][ReadOnly]
    private bool isDashing;
    [SerializeField][ReadOnly]
    private bool isMoving;
    [SerializeField][ReadOnly]
    private bool isAttacking;
    [SerializeField][ReadOnly]
    private bool isAttackEnd;
    [SerializeField][ReadOnly]
    private bool isUsingAbility;

    #region Unity Function
    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 120;
    }
    public void Start()
    {
        timers = GameManager.Instance.TimerManager.GenerateTimers(typeof(coolDownTimers), gameObject);
        timers.times[(int)coolDownTimers.abilityLag].OnTimeIsZero += AbilityLagOver;
        timers.times[(int)coolDownTimers.abilityCast].OnTimeIsZero += AbilityCastOver;
        currentMaxSpeed = maxSpeed;
        currentDashCharges = dashCharges;
        drag = rb.drag;

    }
    #region Updates
    // Update is called once per frame
    void Update()
    {
        StateDecider();
        ExecuteInput();
        DashCounter();
        AimAbility();
        ControllerCursor();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion
    #endregion

    #region GetInputs
    public void SetDirection(InputAction.CallbackContext context)
    {
        if (!context.ReadValue<Vector2>().Equals(Vector2.zero))
        {
            direction = context.ReadValue<Vector2>().normalized;
            LastDirection = direction;
        }
        else
        {
            direction = Vector2.zero;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        GameManager.Instance.CallInteraction();
    }

    public void SetMouseCursor(Transform cursor)
    {
        cursorPos = cursor;
    }
    private Vector2 stickPos;
    public void MousePosition(InputAction.CallbackContext context)
    {
        Vector2 pos = context.ReadValue<Vector2>();
        Debug.Log(GameManager.Instance.currentScheme);
        if (GameManager.Instance.currentScheme == ControlScheme.keyboardAndMouse)    
        {
            Vector3 temp = Camera.main.ScreenToWorldPoint(pos);
            temp.z = 0;
            cursorPos.position = temp;
            mousePos = cursorPos.position;
        }
        else
        {
            stickPos = pos.normalized;
        }
        //if (context.control  )
        //Debug.Log(context.ReadValue<Vector2>() + " " + Input.mousePosition);
    }

    public void ControllerCursor()
    {
        if (GameManager.Instance.currentScheme != ControlScheme.controller)
            return;
        if (stickPos.magnitude > 0)
        {
            cursorPos.position = (Vector2)transform.position + stickPos;
            GameManager.Instance.ShowCursor();
        }
        else
            GameManager.Instance.HideCursor();
        mousePos = cursorPos.position;
    }

    public void BufferLightAttack(InputAction.CallbackContext context)
    {
        BufferInput(actionState.attack);
    }

    public void BufferDash(InputAction.CallbackContext context)
    {
        BufferInput(actionState.dashing);
    }

    public void BufferAbilityOne(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
            BufferInput(actionState.abilityOne);
    }
    public void BufferAbilityTwo(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
            BufferInput(actionState.abilityTwo);
    }
    public void BufferAbilityThree(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
            BufferInput(actionState.abilityThree);
    }

    public void Consume(InputAction.CallbackContext context)
    {
        GameManager.Instance.CallConsume();
    }
    #endregion

    

    #region Ability 

    private void AimAbility()
    {
        Vector3 vectorToTarget = (Vector3)mousePos - AbilityCentre.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        AbilityCentre.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetAbilityState()
    {
        isUsingAbility = true;
        rb.drag = drag * 10;
        timers.SetTime((int)coolDownTimers.abilityCast, abilityCast);
    }

    private void AbilityCastOver(object sender, EventArgs e)
    {
        GoIntoAbilityLag();
    }

    private void GoIntoAbilityLag()
    {
        timers.SetTime((int)coolDownTimers.abilityLag, abilityLag);
        rb.drag = drag;
    }

    private void AbilityLagOver(object sender, EventArgs e)
    {
        StopAbilityLag();
    }

    private void StopAbilityLag()
    {
        isUsingAbility = false;
        timers.ResetSpecificToZero((int)coolDownTimers.abilityLag);
    }
    #endregion

    #region Input Buffering
    private void ExecuteInput()
    {
        switch ((int)bufferedState)
        {
            case (int)actionState.dashing:
                Dash();
                break;
            case (int)actionState.attack:
                PCM.attack.LightAttack();
                break;
            case (int)actionState.abilityOne:
                PCM.abilities.CastSlotOne();
                break;
            case (int)actionState.abilityTwo:
                PCM.abilities.CastSlotTwo();
                break;
            case (int)actionState.abilityThree:
                PCM.abilities.CastSlotThree();
                break;

        }
        if (currentBufferDuration > 0)
            currentBufferDuration--;
        if (currentBufferDuration == 0)
            RemoveBufferInput();
    }

    public void RemoveBufferInput()
    {
        bufferedState = actionState.nothing;
    }

    private void BufferInput(actionState input)
    {
        bufferedState = input;
        currentBufferDuration = bufferDuration;
    }
    #endregion

    #region Movement
    private void Move()
    {
        isMoving = false;
        rb.drag = drag;
        playerState[] allowed = { playerState.idle, playerState.moving };

        if (!CheckStates(allowed))
            return;
        if (rb.velocity.magnitude <= currentMaxSpeed && !direction.Equals(Vector2.zero))
        {
            rb.drag = 0;
            isMoving = true;
            rb.velocity += direction * acceleration * rb.mass;
            if ((rb.velocity + direction * acceleration * rb.mass).magnitude > currentMaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * currentMaxSpeed;
            }
        }
    }
    #endregion

    #region Dash
    private void Dash()
    {
        playerState[] unAllowed = { playerState.attack, playerState.abilityCast };
        if (CheckStates(unAllowed))
            return;
        if (!timers.IsTimeZero((int)coolDownTimers.dashCD) || currentDashCharges < 1)
            return;
        PCM.attack.ResetTimer();
        RemoveBufferInput();
        isDashing = true;
        currentDashCharges--;
        timers.SetTime((int)coolDownTimers.dashCD, dashCDTimer + dashDuration);
        col2D.excludeLayers += enemyLayer;
        dashCoroutine = StartCoroutine(StartDashing());
    }

    private IEnumerator StartDashing()
    {
        float startTime = Time.time;
        Vector2 endPos = (Vector2)transform.position + (LastDirection * dashDistance);
        Vector2 startPos = transform.position;
        for (float timer = 0; timer < dashDuration; timer += Time.deltaTime)
        {
            float ratio = (Time.time - startTime) / dashDuration;
            if (Vector2.Distance(transform.position, endPos) > 0.1f)
            {
                transform.position = Vector2.Lerp(startPos, endPos, ratio);
            }
            else
            {
                transform.position = endPos;
            }
            yield return null;
        }
        StopDash();
    }

    private void DashCounter()
    {
        if (currentDashCharges < dashCharges && CurrentState != playerState.dashing)
        {
            currentDashCharges += (1 / dashRechargeRate) * Time.deltaTime;
        }
        else if (currentDashCharges > dashCharges)
        {
            currentDashCharges = dashCharges;
        }
    }

    private void StopDash()
    {
        if (dashCoroutine != null)
        {
            dashCoroutine = null;
            col2D.excludeLayers -= enemyLayer;
            isDashing = false;
        }
    }

    private void StopDash(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
        StopDash();
    }
    #endregion

    #region Setters
    public void SetIsAttacking(bool isAttack)
    {
        isAttacking = isAttack;
    }

    public void SetIsAttackEnd(bool isEnd)
    {
        isAttackEnd = isEnd;
    }
    #endregion

    #region Utility
    private void StateDecider()
    {
        if (isDashing)
        {
            CurrentState = playerState.dashing;
        }
        else if (isUsingAbility)
        {
            CurrentState = playerState.abilityCast;
        }
        else if (isAttacking)
        {
            CurrentState = playerState.attack;
        }
        else if (isAttackEnd)
        {
            CurrentState = playerState.attackEnd;
        }
        else if (isMoving)
        {
            CurrentState = playerState.moving;
        }
        else
        {
            CurrentState = playerState.idle;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.T_Terrain) && dashCoroutine != null)
        {
            StopDash(dashCoroutine);
        }
    }

    public bool CheckStates(playerState[] allowedStates)
    {
        bool allowed = false;
        foreach (playerState action in allowedStates)
        {
            if (action == CurrentState)
                allowed = true;
        }
        return allowed;
    }
    #endregion
}

