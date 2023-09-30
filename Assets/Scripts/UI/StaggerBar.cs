using System;
using System.Collections;
using System.Collections.Generic;
using KevinCastejon.MoreAttributes;
using UnityEngine;
using UnityEngine.UI;

public class StaggerBar : MonoBehaviour
{
    protected enum StaggerTimer
    {
        staggerDecayTimer
    }

    [field: SerializeField] private Image image;
    [field: SerializeField, ReadOnly] protected float Bar { get; set; }
    [field: SerializeField, ReadOnly] protected int PointsToStagger { get; set; } = 100;
    [field: SerializeField, ReadOnly] protected float StaggerDuration { get; set; } = 3;
    [field: SerializeField, ReadOnly] protected bool Staggered { get; set; } = false;
    [field: SerializeField, ReadOnly] protected float StaggerDelayDuration { get; set; } = 3;
    [field: SerializeField, ReadOnly] protected float StaggerDecayAmount { get; set; } = 4;
    [field: SerializeField, ReadOnly] protected float StaggerDecayRate { get; set; } = 0.25f;
    [field: SerializeField] protected Timer StaggerTimers { get; private set; }
    [field: SerializeField] protected Enemy Parent { get; set; }


    private void Start()
    {
        StaggerTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(StaggerTimer), gameObject);
        StaggerTimers.times[(int)StaggerTimer.staggerDecayTimer].OnTimeIsZero += DecrementStaggerBar;
    }

    public void SetStats(int pointsToStagger, float duration, float delayDuration, float decayAmount, float decayRate)
    {
        PointsToStagger = pointsToStagger;
        StaggerDuration = duration;
        StaggerDelayDuration = delayDuration;
        StaggerDecayAmount = decayAmount;
        StaggerDecayRate = decayRate;
    }

    public void SetToOrange()
    {
        image.color = new Color(255, 162, 0);
    }

    public void SetToPink()
    {
        image.color = new Color(255, 0, 255);
    }

    public void UpdateFillPercent(float percent)
    {
        image.fillAmount = percent;
    }

    private void BeginStagger()
    {
        StartStaggerDecay();
        Staggered = true;
        Parent.BeginStagger();
        SetToPink();
    }

    private void EndStagger()
    {
        Parent.EndStagger();
        Staggered = false;
        SetToOrange();
    }

    private void StartStaggerDecay()
    {
        StaggerTimers.SetTime((int)StaggerTimer.staggerDecayTimer, StaggerDecayRate);
    }

    private void StartDelayedStaggerDecay()
    {
        StaggerTimers.SetTime((int)StaggerTimer.staggerDecayTimer, StaggerDecayRate + StaggerDelayDuration);
    }

    private void DecrementStaggerBar(object sender, EventArgs e)
    {
        if (Bar > 0)
        {
            if (Staggered) 
            {
                Bar -= (PointsToStagger / StaggerDuration) / StaggerDecayAmount;
                if (Bar <= 0) EndStagger();
            }
            else Bar -= StaggerDecayAmount;

            UpdateFillPercent(Bar / PointsToStagger);
            if (Bar > 0) StartStaggerDecay();
        }
    }

    public void AddToStaggerBar(int staggerPoints)
    {
        if (!Staggered)
        {
            Bar += staggerPoints;
            UpdateFillPercent(Bar / PointsToStagger);

            StartDelayedStaggerDecay();

            if (Bar >= PointsToStagger)
            {
                BeginStagger();
            }
        }
    }

    public void ResetStagger()
    {
        Staggered = false;
        Bar = 0;
        StaggerTimers.ResetToZero();
        SetToOrange();
    }
}
