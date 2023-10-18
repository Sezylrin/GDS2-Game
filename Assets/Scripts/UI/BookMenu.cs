using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BookMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject SkillSwitch;
    [SerializeField] private GameObject SkillTree;

    private void Start()
    {
        MainMenu.SetActive(true);
        Settings.SetActive(false);
        SkillSwitch.SetActive(false);
        SkillTree.SetActive(false);
    }
}
