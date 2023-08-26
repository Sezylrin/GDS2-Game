using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blast Ability", menuName = "ScriptableObjects/Blast Ability")]
public class BlastVariantSO : ElementalSO
{
    public Vector2[] initialShape = { new Vector2(0.5f, 0.5f), new Vector2(-0.5f, 0.5f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -0.5f) };

    public Vector2[] finalShape = new Vector2[4];

    public float speed;
}
