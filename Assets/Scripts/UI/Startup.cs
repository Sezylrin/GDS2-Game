using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [SerializeField] private GameObject StartMenu;

    void Start()
    {
        StartMenu.SetActive(true);
        BookMenu bookMenu = GameManager.Instance.BookMenu;
        bookMenu.StartMenuInit();
    }
}
