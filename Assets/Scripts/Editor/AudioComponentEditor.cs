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
            AssetDatabase.Refresh();
            EditorApplication.ExecuteMenuItem("File/Save Project");
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
