using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    public Transform player;
    public PlayerComponentManager PCM;
    private void Awake()
    {
        GameManager.Instance.SetPlayerTransform(player, PCM);
    }
    void Start()
    {
        GameManager.Instance.UIManager.OpenStartMenu();
    }
}
