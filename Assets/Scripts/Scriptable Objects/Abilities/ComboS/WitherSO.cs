using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Combo Effect", menuName = "ScriptableObjects/Combo/Wither Effect")]
public class WitherSO : ComboSO
{
    [Range(1,3)]
    public float[] WitherStrength;
}
