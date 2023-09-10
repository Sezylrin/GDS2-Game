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
    public Attacks attack { get; private set; }
    [field: SerializeField]
    public Abilities abilities { get; private set; }

    private void Start()
    {
        enabled = false;
    }
}
