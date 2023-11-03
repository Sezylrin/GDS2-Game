using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMath
{
    // Start is called before the first frame update
    public static float ClampedDirection(Vector2 originVector, Vector2 dirVector, bool fourMode = true)
    {
        float direction = GetDirection(originVector, dirVector,fourMode);
        return ((fourMode ? 90 : 45) * direction);
    }
    public static int GetDirection(Vector2 originVector, Vector2 dirVector, bool fourMode = true)
    {
        float signedAngle = Vector2.SignedAngle(originVector, dirVector);
        float angle = signedAngle > 0 ? signedAngle : 360f + signedAngle;
        return Mathf.RoundToInt((Mathf.Floor(angle / (fourMode ? 90f : 45f)) + (angle / (fourMode ? 90f : 45f) % 1 > 0.5 ? 1 : 0)) % (fourMode ? 4 : 8));
    }
    public static Vector3 GetEularAngleToDir(Vector2 originVector, Vector2 dirVector)
    {
        return new Vector3(0, 0, Vector2.SignedAngle(originVector, dirVector));
    }
    public static float AngleToDir(Vector2 originVector,Vector2 dir)
    {
        return Vector2.SignedAngle(originVector, dir);
    }

    public static Vector3 CentreOfScreenInUnits()
    {
        return Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
    }

    public static Vector2 RotateByEularAngles(Vector2 Initial, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * Initial;
    }

    public static Quaternion LookAt2D(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
