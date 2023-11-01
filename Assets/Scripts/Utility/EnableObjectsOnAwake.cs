using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectAtStart : MonoBehaviour
{
    public List<GameObject> objectsToWake;

    private void Awake()
    {
        foreach (GameObject obj in objectsToWake)
        {
            if (!obj.activeSelf) obj.SetActive(true);
        }
        Destroy(gameObject);
    }
}