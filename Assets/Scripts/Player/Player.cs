using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static GameObject _instance;
    public static GameObject Instance { get {  return _instance; }}

    private void Awake()
    {
        if (_instance != null && _instance != this.gameObject)
        {
            //Destroy(this.gameObject);
        }
        else
        {
            _instance = this.gameObject;
        }
    }
}