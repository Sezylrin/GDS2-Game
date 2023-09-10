using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Combo Effect", menuName = "ScriptableObjects/Combo/Combo Effect")]
public class ComboSO : ScriptableObject
{

    public float[] BaseDamage;

    public int StaggerDamage;

    public float Duration;
}
