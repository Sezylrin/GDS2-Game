using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(AudioManager))]
public class AudioComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AudioManager manager = (AudioManager)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Load Sounds"))
        {
            string[] guids = AssetDatabase.FindAssets("t:AudioClipSO", new[] { "Assets/Scriptable Objects/Audio" });
            int count = guids.Length;
            AudioClipSO[] clips = new AudioClipSO[count];
            for (int n = 0; n < count; n++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[n]);
                clips[n] = AssetDatabase.LoadAssetAtPath<AudioClipSO>(path);
            }
            
            string enumName = "AudioRef";
            string filePathAndName = "Assets/Scripts/Audio/Enum/" + enumName + ".cs";
            List<string> names = new List<string>();
            for (int i = 0; i < clips.Length; i++)
            {
                if (names.Contains(clips[i].ReferenceName))
                {
                    Debug.LogWarning("There is a duplicate AudioClipSO reference name " + clips[i].ReferenceName + " in the file located at " + AssetDatabase.GUIDToAssetPath(guids[i]));
                    Selection.activeObject = clips[i];
                    //OverrideAudioRefWithDefault();
                    return;
                }
                names.Add(clips[i].ReferenceName);
            }
            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {

                streamWriter.WriteLine("public enum " + enumName);
                streamWriter.WriteLine("{");
                
                for (int i = 0; i < names.Count - 1; i++)
                {
                    streamWriter.WriteLine("	" + clips[i].ReferenceName + ",");
                }
                streamWriter.WriteLine("	" + clips[names.Count - 1].ReferenceName);
                streamWriter.WriteLine("}");
                manager.LoadSounds(clips);
            }

            LoadSoundIntoSO();

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }
    }
    private void LoadSoundIntoSO()
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
    }
    private void OverrideAudioRefWithDefault()
    {
        string enumName = "AudioRef";
        string filePathAndName = "Assets/Scripts/Audio/Enum/" + enumName + ".cs";
        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {

            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");            
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }
}
