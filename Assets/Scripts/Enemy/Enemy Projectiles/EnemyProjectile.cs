using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour
{
    protected enum ProjectileTimer
    {
        lifetimeTimer
    }

    [field: SerializeField] protected float Speed { get; set; } = 1;
    [field: SerializeField] protected float Duration { get; set; } = 3;
    [field: SerializeField] protected Timer ProjectileTimers { get; private set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ProjectileTimers = TimerManager.Instance.GenerateTimers(typeof(ProjectileTimer), gameObject);
        ProjectileTimers.times[(int)ProjectileTimer.lifetimeTimer].OnTimeIsZero += RemoveObject;
        StartLifetime();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        gameObject.transform.Translate(Vector2.right * Speed * Time.deltaTime);
    }

    protected virtual void StartLifetime()
    {
        ProjectileTimers.SetTime((int)ProjectileTimer.lifetimeTimer, Duration);
    }

    protected virtual void RemoveObject(object sender, EventArgs e)
    {
        Destroy(gameObject);
    }
}
