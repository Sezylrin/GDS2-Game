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
}
