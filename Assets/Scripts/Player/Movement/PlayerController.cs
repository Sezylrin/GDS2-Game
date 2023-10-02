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
    abilityLag,
    perfectDodge,
    hit
}
public class PlayerController : MonoBehaviour
{
    private enum coolDownTimers : int
    {
        dashCastCD,
        dashCD,
        abilityCast,
        abilityLag,
        perfectDodge,
        hitStun
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
    [SerializeField, Tooltip("Time before all charges refresh")]
    private float dashRechargeRate;
    [SerializeField] [ReadOnly]
    private int currentDashCharges;

    [Header("Perfect Dodge")]
    [SerializeField]
    private float perfectDodgeDuration;

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

    [Header("Camera")]
    [SerializeField, Range(0f, 8f)]
    private float cameraMouseMin;
    [SerializeField, Range(0f, 16f)]
    private float cameraMouseMax;
    [SerializeField, Range(0,16)]
    private float cameraMaxOffset;
    [field: SerializeField]
    public Transform CameraFollowPoint { get; private set; }
    [Header("Controller Camera")]
    [SerializeField]
    private float camLerpDur;
    [SerializeField, ReadOnly]
    private float currentOffset;
    [SerializeField, Range(0f, 1f)]
    private float snapPoint;
    private Coroutine camLerp;

    [Header("Others")]

    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private LayerMask terrainLayer;
    [SerializeField, ReadOnly, HideOnPlay(true)]
    private Transform cursorPos;
    [SerializeField]
    private LineRenderer lineRend;

    [Header("Debug Values")]

    [SerializeField, ReadOnly]
    private Vector2 direction;
    [SerializeField, ReadOnly]
    private Vector2 lastDirection;
    [field: SerializeField] [field: ReadOnly]
    public playerState CurrentState { get; private set; }
    [SerializeField] [ReadOnly]
    private actionState bufferedState;
    private Vector2 rawPos;
    public Vector2 mousePos { get; private set; }

    private float drag;


    private Coroutine dashCoroutine;
    private Timer timers;
    [SerializeField, ReadOnly]
    private bool isDashing;
    [SerializeField, ReadOnly]
    private bool isMoving;
    [SerializeField, ReadOnly]
    private bool isAttacking;
    [SerializeField, ReadOnly]
    private bool isAttackEnd;
    [SerializeField, ReadOnly]
    private bool isUsingAbility;
    [SerializeField, ReadOnly]
    private bool isPerfectDodge;

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
        timers.times[(int)coolDownTimers.dashCD].OnTimeIsZero += DashResetter;
        timers.times[(int)coolDownTimers.perfectDodge].OnTimeIsZero += StopPerfectDodge;
        currentMaxSpeed = maxSpeed;
        currentDashCharges = dashCharges;
        drag = rb.drag;
        //GameManager.Instance.OnControlSchemeSwitch += SchemeChange;
    }
    #region Updates
    // Update is called once per frame
    void Update()
    {
        StateDecider();
        ExecuteInput();
        UpdateMousePos();
        AimAbility();
        ControllerCursor();
    }

    private void FixedUpdate()
    {
        Move();
        UpdateAimLine();
    }

    #endregion

    #endregion

    #region GetInputs
    public void SetDirection(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>().normalized;
        if (!direction.Equals(Vector2.zero))
        {
            lastDirection = direction;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        GameManager.Instance.CallInteraction();
    }

    public void SetControllerCursor(Transform cursor)
    {
        cursorPos = cursor;
    }
    private Vector2 stickPos;
    public void MousePosition(InputAction.CallbackContext context)
    {
        rawPos = context.ReadValue<Vector2>();
        
    }
    
    private void UpdateMousePos()
    {
        if (GameManager.Instance.currentScheme == ControlScheme.keyboardAndMouse)
        {
            mousePos = Camera.main.ScreenToWorldPoint(rawPos);
            MouseCamFollow();
        }
        else
        {
            stickPos = rawPos.normalized;
        }
    }

    public void ControllerCursor()
    {
        if (GameManager.Instance.currentScheme != ControlScheme.controller)
            return;
        if (stickPos.magnitude > 0)
        {
            cursorPos.position = (Vector2)transform.position + stickPos;
            GameManager.Instance.ShowControllerCursor();
            mousePos = (Vector2)transform.position + stickPos;
        }
        else
        {
            GameManager.Instance.HideControllerCursor();
            mousePos = (Vector2)transform.position + lastDirection;
        }
        SetCamFollowController();
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
        if (context.performed)
            AimLine(0);
        if (context.canceled)
        {
            BufferInput(actionState.abilityOne);
            if (isAim)
            {
                isAim = false;
                StartLerp(0);
            }
        }
    }
    public void BufferAbilityTwo(InputAction.CallbackContext context)
    {
        if (context.performed)
            AimLine(1);
        if (context.canceled)
        {
            BufferInput(actionState.abilityTwo);
            if (isAim)
            {
                isAim = false;
                StartLerp(0);
            }
        }
    }
    public void BufferAbilityThree(InputAction.CallbackContext context)
    {
        if (context.performed)
            AimLine(2);
        if (context.canceled)
        {
            BufferInput(actionState.abilityThree);
            if (isAim)
            {
                isAim = false;
                StartLerp(0);
            }
            
        }
    }
    private bool isAim;
    private int slot;
    private void AimLine(int slot)
    {
        if (PCM.abilities.IsRanged(slot))
        {
            this.slot = slot;
            isAim = true;
            StartLerp(cameraMaxOffset);
        }
    }
    private void UpdateAimLine()
    {
        if (isAim && CheckStates(castState) && PCM.abilities.CanCast(slot))
            lineRend.enabled = true;
        else
            lineRend.enabled = false;

        if (lineRend.enabled)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos - (Vector2)transform.position , float.MaxValue, terrainLayer);
            if (hit.collider != null)
            {
                lineRend.SetPosition(0, transform.position);
                lineRend.SetPosition(1, hit.point);
            }
            else
            {
                lineRend.SetPosition(0, transform.position);
                lineRend.SetPosition(1, transform.position + (Vector3)(mousePos - (Vector2)transform.position).normalized * 20);
            }
        }
    }
    
    
    

    public void Consume(InputAction.CallbackContext context)
    {
        GameManager.Instance.CallConsume();
    }
    #endregion

    #region Camera
    private void MouseCamFollow()
    {
        Vector2 centreOfCam = CustomMath.CentreOfScreenInUnits();
        float mouseDistance = Vector3.Distance(centreOfCam, mousePos);
        Vector3 newCameraPos = transform.position;
        if (mouseDistance > cameraMouseMin)
        {
            float distance = Mathf.Clamp(mouseDistance, cameraMouseMin, cameraMouseMax);
            float scale = (distance - cameraMouseMin) / (cameraMouseMax - cameraMouseMin);
            float offset = Mathf.Lerp(0, cameraMaxOffset, scale);
            newCameraPos += ((Vector3)mousePos - transform.position).normalized * offset;
        }
        CameraFollowPoint.position = newCameraPos;
    }
    private void StartLerp(float desiredOffset)
    {
        if (camLerp != null)
            StopCoroutine(camLerp);
        camLerp = StartCoroutine(LerpCamFollow(desiredOffset));
    }

    private IEnumerator LerpCamFollow(float desiredOffset)
    {
        float startTime = Time.time;
        float startNumber = currentOffset;
        for (float timer = 0; timer < camLerpDur; timer += Time.deltaTime)
        {
            float ratio = (Time.time - startTime) / camLerpDur;
            currentOffset = Mathf.Lerp(startNumber, desiredOffset, ratio);
            if (ratio > snapPoint)
            {
                currentOffset = desiredOffset;
                break;
            }
            yield return null;
        }
        camLerp = null;
    }

    private void SetCamFollowController()
    {
        CameraFollowPoint.position = transform.position + (Vector3)(Vector2)((Vector3)mousePos - transform.position).normalized * currentOffset;
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
        rb.velocity = (mousePos - (Vector2)transform.position).normalized * 0.1f;
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
    private playerState[] castState = { playerState.idle, playerState.moving, playerState.attackEnd, playerState.abilityLag };
    private void ExecuteInput()
    {
        switch ((int)bufferedState)
        {
            case (int)actionState.dashing:
                if (direction.Equals(Vector2.zero))
                    PerfectDodge();
                else
                    Dash();
                break;
            case (int)actionState.attack:
                PCM.attack.LightAttack();
                break;
            case (int)actionState.abilityOne:
                if(CheckStates(castState))
                    PCM.abilities.CastSlotOne();
                break;
            case (int)actionState.abilityTwo:
                if (CheckStates(castState))
                    PCM.abilities.CastSlotTwo();
                break;
            case (int)actionState.abilityThree:
                if (CheckStates(castState))
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
        playerState[] unAllowed = { playerState.attack, playerState.abilityCast, playerState.hit, playerState.perfectDodge };
        if (CheckStates(unAllowed))
            return;
        if (!timers.IsTimeZero((int)coolDownTimers.dashCastCD) || currentDashCharges < 1)
            return;
        PCM.attack.ResetTimer();
        RemoveBufferInput();
        isDashing = true;
        currentDashCharges--;
        timers.SetTime((int)coolDownTimers.dashCastCD, dashCDTimer + dashDuration);
        col2D.excludeLayers += enemyLayer;
        dashCoroutine = StartCoroutine(StartDashing());
        PCM.Trail.DashAfterImage(dashDuration, 5);
    }

    private IEnumerator StartDashing()
    {
        float startTime = Time.time;
        Vector2 endPos = (Vector2)transform.position + (direction * dashDistance);
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

    private void DashResetter(object sender, EventArgs e)
    {
        currentDashCharges = dashCharges;
    }

    private void StopDash()
    {
        timers.SetTime((int)coolDownTimers.dashCD, dashRechargeRate);
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

    #region Perfect Dodge
    private void PerfectDodge()
    {
        playerState[] allowed = { playerState.idle, playerState.attackEnd, playerState.abilityLag };
        if (!CheckStates(allowed))
            return;
        if (isPerfectDodge)
            return;
        RemoveBufferInput();
        isPerfectDodge = true;
        PCM.Trail.PerfectDodge(perfectDodgeDuration, true);
        timers.SetTime((int)coolDownTimers.perfectDodge, perfectDodgeDuration);
        rb.velocity = Vector2.zero;
    }

    private void StopPerfectDodge(object sender, EventArgs e)
    {
        isPerfectDodge = false;
    }

    public void CounteredAttack(float counterQTE)
    {
        PCM.Trail.Countered(counterQTE, true);
        timers.SetTime((int)coolDownTimers.perfectDodge, counterQTE);
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

    public void SetHitStun(float hitStun)
    {
        timers.SetTime((int)coolDownTimers.hitStun, hitStun);
    }
    #endregion

    #region Utility
    private void StateDecider()
    {
        if (!timers.IsTimeZero((int)coolDownTimers.hitStun))
        {
            CurrentState = playerState.hit;
        }
        else if (isPerfectDodge)
        {
            CurrentState = playerState.perfectDodge;
        }
        else if (isDashing)
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
    /// <summary>
    /// returns true is the current state is any of the allowedstates
    /// </summary>
    /// <param name="allowedStates"></param>
    /// <returns></returns>
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

