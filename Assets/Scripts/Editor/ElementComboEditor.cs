using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ElementCombo))]
public class ElementComboEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ElementCombo manager = (ElementCombo)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Load Sounds into SO"))
        {
            string[] guids = AssetDatabase.FindAssets("t:ElementalSO", new[] { "Assets/Scriptable Objects/abilities" });
            int count = guids.Length;
            ElementalSO[] SOs = new ElementalSO[count];
            for (int n = 0; n < count; n++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[n]);
                SOs[n] = AssetDatabase.LoadAssetAtPath<ElementalSO>(path);
            }
            foreach (ElementalSO SO in SOs)
            {
                switch ((int)SO.elementType)
                {
                    case (int)ElementType.wind:
                        SO.audioCast = AudioRef.WindCast;
                        SO.audioHit = AudioRef.WaterHit;
                        break;
                    case (int)ElementType.water:
                        SO.audioCast = AudioRef.WaterCast;
                        SO.audioHit = AudioRef.WaterHit;
                        break;
                    case (int)ElementType.fire:
                        SO.audioCast = AudioRef.FireCast;
                        SO.audioHit = AudioRef.FireHit;
                        break;
                    case (int)ElementType.electric:
                        SO.audioCast = AudioRef.ElectricCast;
                        SO.audioHit = AudioRef.ElectricHit;
                        break;
                }
            }
            AssetDatabase.Refresh();
            EditorApplication.ExecuteMenuItem("File/Save");
        }
    }
}
