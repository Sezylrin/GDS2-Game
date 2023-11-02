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
        staggerDecayTimer,
        delayBetweenStagger
    }

    [field: SerializeField] protected Enemy Parent { get; set; }
    [field: SerializeField] private Image image;
    [field: SerializeField] protected Color StaggeredColour { get; private set; }
    [field: SerializeField] protected Color NotStaggeredColour { get; private set; }
    [field: SerializeField, ReadOnly] protected float Bar { get; set; }
    [field: SerializeField, ReadOnly] protected int PointsToStagger { get; set; } = 100;
    [field: SerializeField, ReadOnly] protected float StaggerMinDuration { get; set; } = 3;
    [field: SerializeField, ReadOnly] protected float StaggerMaxDuration { get; set; } = 3;
    [field: SerializeField, ReadOnly] protected bool Staggered { get; set; } = false;
    [field: SerializeField, ReadOnly] protected float StaggerDelayDuration { get; set; } = 3;
    [field: SerializeField, ReadOnly] protected float StaggerDecayAmount { get; set; } = 4;
    [field: SerializeField, ReadOnly] protected float StaggerDecayRate { get; set; } = 0.25f;
    [field: SerializeField] protected Timer StaggerTimers { get; private set; }
    
    

    protected int DamageToMaxDuration;
    protected int DamageTakenSinceStaggered = 0;
    private void Start()
    {
        StaggerTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(StaggerTimer), gameObject);
        StaggerTimers.times[(int)StaggerTimer.staggerDecayTimer].OnTimeIsZero += DecrementStaggerBar;
    }

    public void SetStats(int pointsToStagger, float minDur, float maxDur, float delayDuration, float decayAmount, float decayRate, int DamageToMax)
    {
        PointsToStagger = pointsToStagger;
        StaggerMinDuration = minDur;
        StaggerMaxDuration = maxDur;
        StaggerDelayDuration = delayDuration;
        StaggerDecayAmount = decayAmount;
        StaggerDecayRate = decayRate;
        DamageToMaxDuration = DamageToMax;
    }

    public void SetToOrange()
    {
        image.color = NotStaggeredColour;
    }

    public void SetToPink()
    {
        image.color = StaggeredColour;
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
        DamageTakenSinceStaggered = 0;
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
                Bar -= (PointsToStagger / StaggerMinDuration) / StaggerDecayAmount;
                if (Bar <= 0)
                {
                    EndStagger();
                    StaggerTimers.SetTime((int)StaggerTimer.delayBetweenStagger, 5);
                }
            }
            else Bar -= StaggerDecayAmount;

            UpdateFillPercent(Bar / PointsToStagger);
            if (Bar > 0) StartStaggerDecay();
        }
    }

    public void AddToStaggerBar(int staggerPoints)
    {
        if (!Staggered && StaggerTimers.IsTimeZero((int)StaggerTimer.delayBetweenStagger))
        {
            Bar = Mathf.Clamp(Bar + staggerPoints, 0, PointsToStagger);
            UpdateFillPercent(Bar / PointsToStagger);

            StartDelayedStaggerDecay();

            if (Bar == PointsToStagger)
            {
                GameManager.Instance.AudioManager.PlaySound(AudioRef.Stagger);
                BeginStagger();
            }
        }
    }

    public void IncreaseStaggerDuration(int damage)
    {
        float extraTime;
        if (DamageTakenSinceStaggered + damage > DamageToMaxDuration)
            damage = DamageToMaxDuration - DamageTakenSinceStaggered;
        DamageTakenSinceStaggered += damage;
        extraTime = Mathf.Clamp((float)damage / (float)DamageToMaxDuration,0,1) * (StaggerMaxDuration - StaggerMinDuration);
        Bar += extraTime * (PointsToStagger / StaggerMinDuration);
        UpdateFillPercent(Bar / PointsToStagger);
    }

    public void ResetStagger()
    {
        Staggered = false;
        Bar = 0;
        StaggerTimers.ResetToZero();
        SetToOrange();
        UpdateFillPercent(0);
    }
}
