using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu(fileName = "AudioClipObj", menuName = "ScriptableObjects/Audio/AudioClipSO")]

public class AudioClipSO : ScriptableObject
{
    // Start is called before the first frame update
    public string ReferenceName;
    public List<AudioClip> clips;
    public AudioMixerGroup mixGroup;
}
