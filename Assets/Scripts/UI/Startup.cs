using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    public Transform player;
    public PlayerComponentManager PCM;
    private void Awake()
    {
        if (GameManager.Instance.PlayerTransform == null)
        {
            Debug.Log("ran");
            GameManager.Instance.SetPlayerTransform(player, PCM);
        }
    }
    void Start()
    {
        GameManager.Instance.UIManager.OpenStartMenu();
    }
}
