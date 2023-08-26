using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KevinCastejon.MoreAttributes;

[System.Serializable]
public class Timer
{
    [field:SerializeField][field:ReadOnly]
    public float[] times { get; private set; }

    public event EventHandler<OnTimeIsZeroEventArgs> OnTimeIsZero;
    public class OnTimeIsZeroEventArgs : EventArgs
    {
        public int timerSlot;
    }

    public GameObject owner;
    /// <summary>
    /// Generate a timer using ints
    /// </summary>
    /// <param name="amountOfTimers"></param>
    /// <param name="owner"></param>
    public Timer(int amountOfTimers, GameObject owner)
    {
        this.owner = owner;
        times = new float[amountOfTimers];
    }
    /// <summary>
    /// Generate Timer using an Enum
    /// </summary>
    /// <param name="enumName"></param>
    /// <param name="owner"></param>
    public Timer(Type enumName, GameObject owner)
    {
        this.owner = owner;
        times = new float[Enum.GetValues(enumName).Length];
    }

    public void InvokeOnTimeIsZero(int timeSlot)
    {
        OnTimeIsZero?.Invoke(this, new OnTimeIsZeroEventArgs { timerSlot = timeSlot });
    }
    /// <summary>
    /// Sets the time in seconds at the int position of the float array
    /// </summary>
    /// <param name="position"></param>
    /// <param name="amount"></param>
    public void SetTime(int position, float amount)
    {
        if (ErrorPosition(position))
        {
           return;
        }
        if (amount <= 0)
        {
            Debug.LogWarning("you have set a redundant time, the timer at position " + position + " will automatically be set to 0");
        }
        times[position] = amount;
    }
    /// <summary>
    /// returns the current time at the int position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float GetTime(int position)
    {
        if (ErrorPosition(position))
        {
            return -1;
        }
        return times[position];
    }
    /// <summary>
    /// returns if the time at the int position is zero
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsTimeZero(int position)
    {
        if (ErrorPosition(position))
        {
            return false;
        }
        return times[position] == 0;
    }
    /// <summary>
    /// use to reduce the time store in index position, useful for cooldown reduction
    /// </summary>
    /// <param name="position"></param>
    /// <param name="amount"></param>
    public void ReduceCoolDown(int position, float amount)
    {
        if (ErrorPosition(position))
        {
            return;
        }
        times[position] -= amount;
    }
    /// <summary>
    /// use to reset all timers back to zero when owner should be deactived or necessary
    /// </summary>
    public void ResetToZero()
    {
        for (int i = 0; i < times.Length; i++)
        {
            times[i] = 0;
        }
    }

    public void DeleteTimer()
    {
        owner = null;
    }

    private bool ErrorPosition(int position)
    {
        if (position >= times.Length || position < 0)
        {
            Debug.Break();
            Debug.LogWarning("Position out of bound, check the position value compared to amount of timers");
            return true;
        }
        return false;
    }
}
