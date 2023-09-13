using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeInteract : InteractionBase
{
    public override void Interact()
    {
        GameManager.Instance.SkillTreeManager.ShowSkillTree();
    }
}
