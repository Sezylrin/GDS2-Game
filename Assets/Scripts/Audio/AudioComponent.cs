using AYellowpaper.SerializedCollections;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    sfx,
    bgm,
}

public class AudioComponent : MonoBehaviour
{
    private List<AudioSource> LoopingSounds = new();

    [SerializeField]
    [SerializedDictionary("Sound Type", "Possible Sounds")]
    private SerializedDictionary<SoundType, AudioClip[]> sounds;

    public void PlaySound(SoundType type, Sound sound = Sound.sfx, int soundOffset = 0)
    {
        if (!sounds.ContainsKey(type))
        {
            Debug.LogError("Sound Type not found in dictionary");
            return;
        }

        AudioClip[] clips = sounds[type];
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.volume = Mathf.Clamp01(GetVolume(sound) + soundOffset / 100f);
        newSource.playOnAwake = false;
        newSource.loop = false;
        newSource.clip = clip;
        newSource.Play();

        StartCoroutine(DestroyAudioSourceWhenFinished(newSource));
    }

    public void PlayLoopingSound(SoundType type, Sound sound = Sound.sfx, int soundOffset = 0)
    {
        if (!sounds.ContainsKey(type))
        {
            Debug.LogWarning("Sound Type not found in dictionary");
            return;
        }

        AudioClip[] clips = sounds[type];
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.volume = Mathf.Clamp01(GetVolume(sound) + soundOffset / 100f);
        newSource.playOnAwake = false;
        newSource.loop = true;
        newSource.clip = clip;
        newSource.Play();

        LoopingSounds.Add(newSource);
    }

    private float GetVolume(Sound sound)
    {
        switch (sound)
        {
            case Sound.sfx:
                return GameManager.Instance.AudioManager.sfxVolume;
            case Sound.bgm:
                return GameManager.Instance.AudioManager.bgmVolume;
            default:
                return 0;
        }
    }

    public void DestroyLoopingSound(SoundType type)
    {
        AudioSource toDestroy = null;
        foreach (AudioSource source in LoopingSounds)
        {
            if (source.clip != null && Array.Exists(sounds[type], clip => clip == source.clip))
            {
                toDestroy = source;
                break;
            }
        }

        if (toDestroy != null)
        {
            LoopingSounds.Remove(toDestroy);
            Destroy(toDestroy);
        }
        else
        {
            Debug.LogWarning("No looping sound found of specified type");
        }
    }

    private IEnumerator DestroyAudioSourceWhenFinished(AudioSource source)
    {
        yield return new WaitUntil(() => !source.isPlaying);

        Destroy(source);
    }
}
