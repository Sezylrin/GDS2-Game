using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KevinCastejon.MoreAttributes;
public class SkillPointUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Image skillImage;

    public void EnableSkillPoint()
    {
        skillImage.enabled = true;
    }

    public void DisableSkillPoint()
    {
        skillImage.enabled = false;
    }
}
