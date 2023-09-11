using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KevinCastejon.MoreAttributes;

[Serializable]
public class Timer
{
    [field:SerializeField][field:ReadOnly]
    public Times[] times { get; private set; }

    [Serializable]
    public struct Times
    {
        [SerializeField][HideInInspector]
        private string name;
        [ReadOnly]
        public float time;
        public EventHandler OnTimeIsZero;
        [ReadOnly]
        public float setTime;
        public void SetName(string name)
        {
            this.name = name;
        }
    }

    [ReadOnly]
    public GameObject owner;
    /// <summary>
    /// Generate a timer using ints
    /// </summary>
    /// <param name="amountOfTimers"></param>
    /// <param name="owner"></param>
    public Timer(int amountOfTimers, GameObject owner)
    {
        this.owner = owner;
        times = new Times[amountOfTimers];
        for (int i = 0; i < times.Length; i++)
        {
            times[i].SetName("timer " + i.ToString());
        }
    }
    /// <summary>
    /// Generate Timer using an Enum
    /// </summary>
    /// <param name="enumName"></param>
    /// <param name="owner"></param>
    public Timer(Type enumName, GameObject owner)
    {
        this.owner = owner;
        int length = Enum.GetValues(enumName).Length;
        times = new Times[length];
        for (int i = 0; i < length; i++)
        {
            times[i].SetName(Enum.GetName(enumName, i));
        }
    }

    public void SetName(int position, string name)
    {
        if (ErrorPosition(position, "SetName"))
        {
            return;
        }
        times[position].SetName(name);
    }

    public void InvokeOnTimeIsZero(int timeSlot)
    {
        times[timeSlot].OnTimeIsZero?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Sets the time in seconds at the int position of the float array
    /// </summary>
    /// <param name="position"></param>
    /// <param name="amount"></param>
    public void SetTime(int position, float amount)
    {
        if (ErrorPosition(position,"SetTime"))
        {
           return;
        }
        if (amount == 0)
        {
            InvokeOnTimeIsZero(position);
        }
        times[position].time = amount;
        times[position].setTime = amount;
    }
    /// <summary>
    /// returns the current time at the int position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float GetTime(int position)
    {
        if (ErrorPosition(position,"GetTime"))
        {
            return -1;
        }
        return times[position].time;
    }
    /// <summary>
    /// returns if the time at the int position is zero
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsTimeZero(int position)
    {
        if (ErrorPosition(position,"IsTimeZero"))
        {
            return false;
        }
        return times[position].time == 0;
    }
    /// <summary>
    /// use to reduce the time store in index position, useful for cooldown reduction
    /// </summary>
    /// <param name="position"></param>
    /// <param name="amount"></param>
    public void ReduceCoolDown(int position, float amount)
    {
        if (ErrorPosition(position,"ReduceCoolDown"))
        {
            return;
        }
        times[position].time -= amount;
    }
    /// <summary>
    /// use to reset all timers back to zero when owner should be deactived or necessary
    /// </summary>
    public void ResetToZero()
    {
        for (int i = 0; i < times.Length; i++)
        {
            times[i].time = 0;
        }
    }

    public void ResetSpecificToZero(int position)
    {
        if (ErrorPosition(position, "ResetSpecificToZero"))
        {
            return;
        }
        times[position].time = 0;
    }

    public float RatioOfTimePassed(int position)
    {
        if (ErrorPosition(position,"RatioOfTimePassed"))
        {
            return 0;
        }
        if (times[position].setTime == 0)
        {
            return 1;
        }
        else
        {
            return 1 - (times[position].time / times[position].setTime);
        }
    }

    public void DeleteTimer()
    {
        owner = null;
    }

    private bool ErrorPosition(int position, string var)
    {
        if (position >= times.Length || position < 0)
        {
            Debug.Break();
            Debug.LogWarning(var +" Call's position is out of bound, check the position value compared to amount of timers");
            return true;
        }
        return false;
    }
}
