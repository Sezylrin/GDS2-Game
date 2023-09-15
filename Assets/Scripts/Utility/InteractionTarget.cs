using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTarget : MonoBehaviour
{
    public void OnInteractedWith()
    {
        GameManager.Instance.SkillTreeManager.ShowSkillTree();
    }
}
