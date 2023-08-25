using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMath
{
    // Start is called before the first frame update
    public static float ClampedDirection(Vector2 originVector, Vector2 dirVecotr)
    {
        float signedAngle = Vector2.SignedAngle(originVector, dirVecotr);
        float angle = signedAngle > 0 ? signedAngle : 360f + signedAngle;
        float direction = (Mathf.Floor(angle / 90f) + (angle / 90f % 1 > 0.5 ? 1 : 0)) % 4;
        return (90 * direction);
    }
}
