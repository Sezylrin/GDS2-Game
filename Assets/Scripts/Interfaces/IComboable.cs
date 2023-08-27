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
    public void SetTimers();
    public void ApplyFireSurge();
    public void ApplyAquaVolt();
    public void ApplyFireTornado(LayerMask Target);
    public void ApplyBrambles(LayerMask Target);
    public void ApplyNoxiousGas();
    public void ApplyWither();

}
