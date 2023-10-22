using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponentManager : MonoBehaviour
{
    [field:SerializeField]
    public InputController input { get; private set; }
    [field: SerializeField]
    public PlayerController control { get; private set; }
    [field: SerializeField]
    public PlayerSystem system { get; private set; }
    [field: SerializeField]
    public Abilities abilities { get; private set; }
    [field: SerializeField]
    public PlayerUI UI{ get; private set; }
    [field: SerializeField]
    public PlayerTrailEffect Trail { get; private set; }

    public static GameObject Instance { get; private set; }

    private void Start()
    {
        
        enabled = false;
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            Instance.gameObject.SetActive(true);
        }
        else
        {
            Instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }
}
