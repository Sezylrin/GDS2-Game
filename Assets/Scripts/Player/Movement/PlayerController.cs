using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KevinCastejon.MoreAttributes;

public enum actionState
{
    idle,
    moving,
    dashing,
    attack,
    attackEnd
}
public class PlayerController : MonoBehaviour
{
    private enum coolDownTimers : int
    {
        dashCD
    }
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

    [Header("Input Buffer")]

    [SerializeField]
    private int bufferDuration;
    private int currentBufferDuration;

    [field: Header("Core variables")]
    [field: SerializeField]
    public Rigidbody2D rb { get; private set; }
    [SerializeField]
    private CircleCollider2D col2D;
    [SerializeField]
    private Attacks attack;

    [Header("Others")]

    [SerializeField]
    private LayerMask enemyLayer;


    [Header("Debug Values")]

    [SerializeField] [ReadOnly]
    private Vector2 direction;
    [SerializeField] [ReadOnly]
    private Vector2 LastDirection;
    [field: SerializeField] [field: ReadOnly]
    public actionState CurrentState { get; private set; }
    [SerializeField] [ReadOnly]
    private actionState bufferedState;
    public Vector2 mousePos { get; private set; }

    private float drag;

    private Camera cam;

    private Coroutine dashCoroutine;
    private Timer timers;
    [SerializeField] [ReadOnly]
    private bool isDashing;
    [SerializeField] [ReadOnly]
    private bool isMoving;
    [SerializeField] [ReadOnly]
    private bool isAttacking;
    [SerializeField] [ReadOnly]
    private bool isAttackEnd;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 120;
    }
    public void Start()
    {
        CurrentState = actionState.idle;
        timers = TimerManager.instance.GenerateTimers(typeof(coolDownTimers), gameObject);
        CurrentState = actionState.idle;
        currentMaxSpeed = maxSpeed;
        cam = Camera.main;
        currentDashCharges = dashCharges;
        drag = rb.drag;
    }

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

    private void SetMousePos()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void BufferLightAttack(InputAction.CallbackContext context)
    {
        BufferInput(actionState.attack);
    }

    public void BufferDash(InputAction.CallbackContext context)
    {
        BufferInput(actionState.dashing);
    }
    #endregion

    #region Updates
    // Update is called once per frame
    void Update()
    {
        StateDecider();
        SetMousePos();
        ExecuteInput();
        DashCounter();
    }

    private void FixedUpdate()
    {
        Move();
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
                attack.LightAttack();
                break;

        }
        if (currentBufferDuration > 0)
            currentBufferDuration--;
        if (currentBufferDuration == 0)
            RemoveBufferInput();
    }

    public void RemoveBufferInput()
    {
        bufferedState = actionState.idle;
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
        actionState[] allowed = { actionState.idle, actionState.moving };

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
        actionState[] unAllowed = { actionState.attack };
        if (CheckStates(unAllowed))
            return;
        if (!timers.IsTimeZero((int)coolDownTimers.dashCD) || currentDashCharges < 1)
            return;
        attack.ResetTimer();
        RemoveBufferInput();
        isDashing = true;
        currentDashCharges--;
        timers.SetTime((int)coolDownTimers.dashCD, dashCDTimer + dashDuration);
        CurrentState = actionState.dashing;
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
        if (currentDashCharges < dashCharges && CurrentState != actionState.dashing)
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
            CurrentState = actionState.dashing;
        }
        else if (isAttacking)
        {
            CurrentState = actionState.attack;
        }
        else if (isAttackEnd)
        {
            CurrentState = actionState.attackEnd;
        }
        else if (isMoving)
        {
            CurrentState = actionState.moving;
        }
        else
        {
            CurrentState = actionState.idle;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            StopDash(dashCoroutine);
        }
    }

    public bool CheckStates(actionState[] allowedStates)
    {
        bool allowed = false;
        foreach (actionState action in allowedStates)
        {
            if (action == CurrentState)
                allowed = true;
        }
        return allowed;
    }
    #endregion
}

