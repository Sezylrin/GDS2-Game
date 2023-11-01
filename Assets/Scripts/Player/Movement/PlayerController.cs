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
    abilityThree,
    abilityFour
}
public enum playerState
{
    idle,
    moving,
    dashing,
    //attack,
    //attackEnd,
    abilityStart,
    abilityCast,
    abilityLag,
    perfectDodge,
    hit,
    consuming
}
public class PlayerController : MonoBehaviour
{
    private enum coolDownTimers : int
    {
        dashCastCD,
        dashCD,
        abilitystart,
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
    private CircleCollider2D circCol2D;
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

    [field: Header("Debug Values")]

    [field: SerializeField, ReadOnly]
    public Vector2 direction { get; private set; }
    [field: SerializeField, ReadOnly]
    public Vector2 lastDirection { get; private set; }
    [field: SerializeField, ReadOnly]
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
    /*[SerializeField, ReadOnly]
    private bool isAttacking;
    [SerializeField, ReadOnly]
    private bool isAttackEnd;*/
    [SerializeField, ReadOnly]
    private bool isStartingAbility;
    [SerializeField, ReadOnly]
    private bool isUsingAbility;
    [SerializeField, ReadOnly]
    private bool isAbilityLag;
    [SerializeField, ReadOnly]
    private bool isPerfectDodge;

    private LayerMask initialLayer;
    #region Unity Function
    void Awake()
    {
        lastDirection = Vector2.up;
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
        timers.times[(int)coolDownTimers.abilitystart].OnTimeIsZero += CastAbility;
        currentMaxSpeed = maxSpeed;
        currentDashCharges = dashCharges;
        drag = rb.drag;
        initialLayer = circCol2D.excludeLayers;
        GameManager.Instance.SetCameraTrack(CameraFollowPoint);
        //GameManager.Instance.OnControlSchemeSwitch += SchemeChange;
    }
    #region Updates
    // Update is called once per frame
    void Update()
    {
        StateDecider();
        UpdateLastDirection();
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
    }  

    public void SetLastDir(Vector2 newlastDir)
    {
        lastDirection = newlastDir;
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
    public void BufferAbilityFour(InputAction.CallbackContext context)
    {
        if (context.performed)
            AimLine(3);
        if (context.canceled)
        {
            BufferInput(actionState.abilityFour);
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
        if (isAim && CheckStates(castState))
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
        Vector2 vectorToTarget;
        if (mousePos != Vector2.zero)
            vectorToTarget = mousePos - (Vector2)AbilityCentre.position;
        else
            vectorToTarget = lastDirection;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        AbilityCentre.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void StartAbility(float startSpeed)
    {
        isStartingAbility = true;
        rb.velocity = (mousePos - (Vector2)transform.position).normalized * 0.1f;
        timers.SetTime((int)coolDownTimers.abilitystart, startSpeed);
    }

    public void CastAbility(object sender, EventArgs e)
    {
        isStartingAbility = false;
        if (CurrentState != playerState.abilityStart)
            return;
        isUsingAbility = true;
        float castDur;
        PCM.abilities.CastAbility(out castDur);
        timers.SetTime((int)coolDownTimers.abilityCast, castDur);
    }

    private void AbilityCastOver(object sender, EventArgs e)
    {
        isUsingAbility = false;
        isAbilityLag = true;
        GoIntoAbilityLag();
        
    }

    private void GoIntoAbilityLag()
    {
        timers.SetTime((int)coolDownTimers.abilityLag, abilityLag);
        rb.drag = drag;
    }

    private void AbilityLagOver(object sender, EventArgs e)
    {
        isAbilityLag = false;
    }

    #endregion

    #region Input Buffering
    private playerState[] castState = { playerState.idle, playerState.moving };
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
            case (int)actionState.abilityFour:
                if (CheckStates(castState))
                    PCM.abilities.CastSlotFour();
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
    private playerState[] allowedLastDir = { playerState.moving, playerState.idle, playerState.abilityLag };
    private void UpdateLastDirection()
    {
        if (!CheckStates(allowedLastDir))
            return;

        if (!direction.Equals(Vector2.zero) && !lastDirection.Equals(direction))
        {
            lastDirection = direction;
        }
    }
    private void Move()
    {
        isMoving = false;
        rb.drag = drag;
        if (CurrentState == playerState.abilityStart)
            rb.drag = drag * 0.5f;
        playerState[] allowed = { playerState.idle, playerState.moving,  playerState.abilityLag };

        if (!CheckStates(allowed))
        {
            //rb.velocity = Vector2.zero;
            return;
        }
        float tempMaxSpeed = currentMaxSpeed;
        if (CurrentState == playerState.abilityStart)
        {
            tempMaxSpeed *= 0.5f;
        }
        if (rb.velocity.magnitude <= tempMaxSpeed && !direction.Equals(Vector2.zero))
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
        playerState[] unAllowed = { playerState.abilityCast, playerState.hit, playerState.perfectDodge, playerState.consuming };
        if (CheckStates(unAllowed))
            return;
        if (!timers.IsTimeZero((int)coolDownTimers.dashCastCD) || currentDashCharges < 1)
            return;
        RemoveBufferInput();
        isDashing = true;
        currentDashCharges--;
        timers.SetTime((int)coolDownTimers.dashCastCD, dashCDTimer + dashDuration);

        GameManager.Instance.AudioManager.PlaySound(AudioRef.Dash, false, 0.65f);
        BeginDash(dashDistance, dashDuration, direction);
    }

    public void Dash(float distance, float dur, Vector2 dir, Color color, float blend)
    {
        dashCoroutine = StartCoroutine(StartDashing(distance, dur, dir));
        PCM.Trail.DashAfterImage(dur, 5, color, blend);
    }

    private void BeginDash(float distance, float dur, Vector2 dir)
    {
        col2D.excludeLayers += enemyLayer;
        circCol2D.excludeLayers += enemyLayer;
        dashCoroutine = StartCoroutine(StartDashing(distance, dur, dir));
        PCM.Trail.DashAfterImage(dur, 5, Color.white, 0);
    }

    private IEnumerator StartDashing(float distance, float dur, Vector2 dir)
    {
        float startTime = Time.time;
        Vector2 endPos = (Vector2)transform.position + (dir * distance);
        Vector2 startPos = transform.position;
        Vector2 dashDirection = endPos - startPos;
        for (float timer = 0; timer < dur; timer += Time.deltaTime)
        {
            
            float ratio = (Time.time - startTime) / dur;
            //float cubic = Mathf.Sin((ratio * Mathf.PI) * 0.5f);
            Vector2 nextPosition = Vector2.Lerp(startPos, endPos, ratio);
            if (Physics2D.CapsuleCast(transform.position, col2D.size, CapsuleDirection2D.Vertical, 0, dashDirection,Vector2.Distance(transform.position,nextPosition),terrainLayer))
            {
                break;
            }
            if (Vector2.Distance(transform.position, endPos) > 0.1f)
            {                
                transform.position = nextPosition;
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
        }
        col2D.excludeLayers = 0;
        circCol2D.excludeLayers = initialLayer;
        isDashing = false;
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
        playerState[] allowed = { playerState.idle, playerState.abilityLag, playerState.abilityStart };
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
    #endregion

    #region Setters Getter
    public void SetIsAttacking(bool isAttack)
    {
        //isAttacking = isAttack;
    }

    public void SetIsAttackEnd(bool isEnd)
    {
        //isAttackEnd = isEnd;
    }

    public void SetHitStun(float hitStun)
    {
        timers.SetTime((int)coolDownTimers.hitStun, hitStun);
    }

    public bool GetAbilityLag()
    {
        return timers.IsTimeZero((int)coolDownTimers.abilityLag);
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
        else if (isStartingAbility)
        {
            CurrentState = playerState.abilityStart;
        }
        else if (isUsingAbility)
        {
            CurrentState = playerState.abilityCast;
        }
        else if (isAbilityLag)
        {
            CurrentState = playerState.abilityLag;
        }
        /*else if (isAttacking)
        {
            CurrentState = playerState.attack;
        }
        else if (isAttackEnd)
        {
            CurrentState = playerState.attackEnd;
        }*/
        else if (isMoving)
        {
            CurrentState = playerState.moving;
        }
        else
        {
            CurrentState = playerState.idle;
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

