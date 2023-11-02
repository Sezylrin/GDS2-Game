using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ButtonPromptSelector : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Dont Touch")]
    [SerializeField] private bool isCanvas;
    [SerializeField] private ButtonPrompts promptsSO;
    [SerializeField] private Image canvasImage;
    [SerializeField] private SpriteRenderer spriteImage;
    [Space(20), Header("Prompt SetUp")]
    [SerializeField, Tooltip("Equivelant Keyboard Image")] private Sprite KeyboardSprite;
    [SerializeField, Tooltip("Equivelant Controller Image")] private ControllerButtons controllerButton;
    [Space(10), Header("Preview")]
    [SerializeField] private ControlScheme previewScheme;
    [SerializeField] private ControllerScheme previewController;
    void Start()
    {
    }
    private void OnDisable()
    {
        GameManager.Instance.OnControlSchemeSwitch -= SwitchPrompt;
    }
    private void OnEnable()
    {
        if(GameManager.Instance)            
        {
            SetPrompt(GameManager.Instance.currentScheme, GameManager.Instance.controllerType);
            GameManager.Instance.OnControlSchemeSwitch += SwitchPrompt;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwitchPrompt(object sender, EventArgs e)
    {
        SetPrompt(GameManager.Instance.currentScheme, GameManager.Instance.controllerType);
    }

    private void SetPrompt(ControlScheme newScheme, ControllerScheme whichController)
    {
        Debug.Log("NotError");
        Debug.Log(gameObject.name);
        if (newScheme == ControlScheme.keyboardAndMouse)
        {
            if (isCanvas)
                canvasImage.sprite = KeyboardSprite;
            else
                spriteImage.sprite = KeyboardSprite;
        }
        else
        {
            Sprite sprite = promptsSO.Prompts[controllerButton][whichController];
            if (isCanvas)
                canvasImage.sprite = sprite;
            else
                spriteImage.sprite = sprite;
        }
        Debug.Log("error");
    }

    public void Preview()
    {
        if (isCanvas)
        {
            if (!canvasImage)
                canvasImage = GetComponent<Image>();
        }
        else
        {
            if (!spriteImage)
                spriteImage = GetComponent<SpriteRenderer>();
        }
        SetPrompt(previewScheme,previewController);
    }
}
