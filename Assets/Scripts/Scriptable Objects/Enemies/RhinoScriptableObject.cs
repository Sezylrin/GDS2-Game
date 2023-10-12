using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RhinoSO", menuName = "ScriptableObjects/Enemies/RhinoSO")]
public class RhinoScriptableObject : BaseEnemyScriptableObject
{
    [Space(20)]
    public float ChargeSpeed = 2;
    public float attack2Range = 3;
    public float shockwaveStartRadius = 0.13f;
    public float shockwaveEndRadius;
    public float shockwaveGrowthSpeed;
    public float stompMoveSpeed = 3;
    
}

