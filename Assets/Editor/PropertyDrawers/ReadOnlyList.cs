using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReadOnlyList : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true); // Disable editing
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }
}
