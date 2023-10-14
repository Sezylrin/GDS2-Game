using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SnakeSO", menuName = "ScriptableObjects/Enemies/SnakeSO")]
public class SnakeScriptableObject : BaseEnemyScriptableObject
{
    [Space(20)]

    public float SingleShotSpeed;
    public int RapidFireAmount;
    public float RapidFireSpeed;
    public float AcidBlobSpeed;
}
