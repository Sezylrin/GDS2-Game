using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Area Combo Effect", menuName = "ScriptableObjects/Combo/Area Combo")]
public class AreaComboSO : ComboSO
{
    public float radius;

    public ElementType typeOne;

    public ElementType typeTwo;

    public float damageTickRate;
}
