using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
[CreateAssetMenu(fileName = "AudioClipObj", menuName = "ScriptableObjects/Audio/LoadedAudio")]
public class LoadedSoundDict : ScriptableObject
{
    public SerializedDictionary<string, AudioClipSO> loadedSound;
}
