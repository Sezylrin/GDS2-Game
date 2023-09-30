using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMath
{
    // Start is called before the first frame update
    public static float ClampedDirection(Vector2 originVector, Vector2 dirVector, bool fourMode = true)
    {
        float signedAngle = Vector2.SignedAngle(originVector, dirVector);
        float angle = signedAngle > 0 ? signedAngle : 360f + signedAngle;
        float direction = (Mathf.Floor(angle / (fourMode? 90f : 45f)) + (angle / (fourMode ? 90f : 45f) % 1 > 0.5 ? 1 : 0)) % (fourMode ? 4 : 8);
        return ((fourMode ? 90 : 45) * direction);
    }

    public static Vector3 CentreOfScreenInUnits()
    {
        return Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
    }
}
