using System;
using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    protected enum EnemyManagerTimer
    {
        attackDelayTimer,
        attackPointRefeshTimer
    }

    public static EnemyManager Instance { get; set; }

    [field: Header("Something")]
    [field: SerializeField, ReadOnly] private int AttackPoints { get; set; }
    [field: SerializeField] private int MaxAttackPoints { get; set; } = 10;
    [field: SerializeField] private float AttackPointRefreshRate { get; set; } = 2;
    [field: SerializeField] private bool CanEnemyAttack { get; set; } = true;
    [field: SerializeField] private float AttackDelay { get; set; } = 0.5f;
    [field: SerializeField] private Timer EnemyManagerTimers { get; set; }

    [field: SerializeField, ReadOnly] private int ActiveEnemies { get; set; }
    [field: SerializeField, ReadOnly] private int TotalEnemiesThisRoom { get; set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyManagerTimers = TimerManager.Instance.GenerateTimers(typeof(EnemyManagerTimer), gameObject);
        EnemyManagerTimers.times[(int)EnemyManagerTimer.attackDelayTimer].OnTimeIsZero += EnableAttack;
        EnemyManagerTimers.times[(int)EnemyManagerTimer.attackPointRefeshTimer].OnTimeIsZero += RefreshAttackPoint;

        AttackPoints = MaxAttackPoints;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CanAttack()
    {
        return CanEnemyAttack;
    }

    public void ManagerAttack()
    {
        if (AttackPoints == MaxAttackPoints) StartAttackPointRefresh();
        AttackPoints--;
        CanEnemyAttack = false;
        EnemyManagerTimers.SetTime((int)EnemyManagerTimer.attackDelayTimer, AttackDelay);
    }

    private void EnableAttack(object sender, EventArgs e)
    {
        CanEnemyAttack = true;
    }

    private void StartAttackPointRefresh()
    {
        EnemyManagerTimers.SetTime((int)EnemyManagerTimer.attackPointRefeshTimer, AttackPointRefreshRate);
    }

    private void RefreshAttackPoint(object sender, EventArgs e)
    {
        if (AttackPoints < MaxAttackPoints) AttackPoints++;
        if (AttackPoints < MaxAttackPoints) StartAttackPointRefresh();
    }

    public void DecrementActiveEnemyCounter()
    {
        ActiveEnemies--;
    }
}
