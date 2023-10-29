using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    public Transform player;
    public PlayerComponentManager PCM;
    void Start()
    {
        GameManager.Instance.UIManager.OpenStartMenu();
        GameManager.Instance.SetPlayerTransform(player,PCM);
    }
}
