using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ButtonPromptSelector))]
public class ButtonPromptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ButtonPromptSelector prompt = (ButtonPromptSelector)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Preview Prompt"))
        {
            prompt.Preview();
        }
    }
}
