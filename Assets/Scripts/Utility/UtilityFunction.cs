using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunction
{
    public static bool FindComponent<T>(Transform transform, out T result)
    {
        while (transform != null)
        {
            if (transform.TryGetComponent(out result)) return true;
            transform = transform.parent;
        }
        result = default;
        return false;
    }

    private static Quaternion LookAt2D(Vector3 origin, Vector3 lookTarget)
    {
        Vector3 vectorToTarget = (Vector3)lookTarget - origin;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
