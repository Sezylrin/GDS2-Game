using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComboable
{
    public Transform SpawnPosition { get; set; }
    public List<ElementCombos> ActiveCombos { get; set; }
    public Timer ComboEffectTimer { get; set; }
    public LayerMask TargetLayer { get; set; }
    public float CurrentWitherBonus { get; set; }
    public bool IsNoxious { get; set; }
    public bool IsWither { get; set; }
    public bool IsBrambled { get; set; }
    public bool IsStunned { get; set; }
    public void SetTimers();
    public void ApplyFireSurge(float damage, int stagger);
    public void ApplyAquaVolt(float damage, int stagger, float duration);
    public void RemoveStun();
    public void ApplyNoxiousGas(float damage, int stagger, float duration);
    public void RemoveNoxious();
    public void ApplyWither(float damage, int stagger, float duration, float witherBonus);
    public void RemoveWither();

}
