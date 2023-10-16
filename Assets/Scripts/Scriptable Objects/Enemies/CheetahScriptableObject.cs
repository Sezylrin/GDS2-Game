using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheetahSO", menuName = "ScriptableObjects/Enemies/CheetahSO")]
public class CheetahScriptableObject : BaseEnemyScriptableObject
{
    [Space(20)]
    public float swipeRange;
    public float swipeDistance;
    public float swipeDuration;
}
