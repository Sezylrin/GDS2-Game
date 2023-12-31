using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using AYellowpaper.SerializedCollections;
using System;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
public class AudioManager : MonoBehaviour
{
    public float masterVolume = 100;
    public float sfxVolume = 100;
    public float bgmVolume = 100;

    [SerializeField, Tooltip("Loaded Sounds, press Load sounds to load in sounds, do not edit the dictionary")]
    [SerializedDictionary("Reference", "AudioClipSO")]
    private SerializedDictionary<string, AudioClipSO> LoadedAudioClips;
    [SerializeField] AudioMixerGroup SFXMixerGroup;
    [SerializeField] AudioMixerGroup BGMMixerGroup;
    [SerializeField] AudioMixerGroup MasterMixerGroup;
    [SerializeField]
    private Stack<AudioObj> availableSources = new Stack<AudioObj>();
    [SerializeField]
    private GameObject audioObjPF;
    [SerializeField]
    private LoadedSoundDict loadedAudio;
    private void Start()
    {
        UpdateDict(loadedAudio);
    }
#if UNITY_EDITOR
    
    public void LoadSounds(AudioClipSO[] newList)
    {
        string[] guids = AssetDatabase.FindAssets("t:LoadedSoundDict", new[] { "Assets/Scriptable Objects/Audio/LoadedSound" });
        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Debug.Log(path);
        LoadedSoundDict loadedSO = AssetDatabase.LoadAssetAtPath<LoadedSoundDict>(path);
        loadedSO.loadedSound.Clear();
        foreach (AudioClipSO clip in newList)
        {
            loadedSO.loadedSound.Add(clip.ReferenceName, clip);
        }
        UpdateDict(loadedSO);
        EditorUtility.SetDirty(loadedSO);
    }
#endif
    private void UpdateDict(LoadedSoundDict loadedSO)
    {
        LoadedAudioClips.Clear();
        foreach(KeyValuePair<string,AudioClipSO> obj in loadedSO.loadedSound)
        {
            LoadedAudioClips.Add(obj.Key, obj.Value);
        }
    }
    public AudioObj PlaySound(string reference, bool loop = false, float volume = 1)
    {
        AudioClipSO SO = loadedAudio.loadedSound[reference];
        AudioClip clipToPlay = SO.clips[UnityEngine.Random.Range(0,SO.clips.Count)];
        AudioObj temp;
        if (availableSources.Count > 0)
        {
            temp = availableSources.Pop();            
        }
        else
        {
            temp = Instantiate(audioObjPF, transform).GetComponent<AudioObj>();
            temp.Init(this);
        }
        temp.StartPlaying(clipToPlay, SO.mixGroup, loop, volume);
        return temp;
    }

    public AudioObj PlaySound(AudioRef reference, bool loop = false, float volume = 1)
    {
        return PlaySound(reference.ToString(), loop, volume);
    }

    public void ReAddToStack(AudioObj obj)
    {
        availableSources.Push(obj);
    }

    private float LinearToDecibel(float linear)
    {
        if (linear != 0)
            return 20.0f * Mathf.Log10(linear);
        else
            return -80.0f;
    }

    public void ModifyBGMVolume(float volume)
    {
        bgmVolume = volume;
        BGMMixerGroup.audioMixer.SetFloat("BGM_Volume", LinearToDecibel(bgmVolume / 100f));
    }

    public void ModifyMasterVolume(float volume)
    {
        masterVolume = volume;
        MasterMixerGroup.audioMixer.SetFloat("Master_Volume", LinearToDecibel(masterVolume / 100f));
    }

    public void ModifySFXVolume(float volume)
    {
        sfxVolume = volume;
        SFXMixerGroup.audioMixer.SetFloat("SFX_Volume", LinearToDecibel(sfxVolume / 100f));
    }
}
