using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private Dictionary<AudioRef, AudioObj> ActiveSounds = new();

    public void PlayMusic(AudioRef audioRef, bool overrideCurrentSounds = false, bool loop = true)
    {
        if (overrideCurrentSounds)
        {
            StopAllMusic();
        }

        AudioObj playingSound = GetAudioManager().PlaySound(audioRef, loop);
        ActiveSounds.Add(audioRef, playingSound);
    }

    public void PlayMultiple(AudioRef[] sounds, bool overrideCurrentSounds = false, bool loop = true)
    {
        if (overrideCurrentSounds)
        {
            StopAllMusic();
        }

        foreach (AudioRef sound in sounds)
        {
            AudioObj playingSound = GetAudioManager().PlaySound(sound, loop);
            ActiveSounds.Add(sound, playingSound);
        }
    }

    public void PauseMusic(AudioRef audioRef)
    {
        if (ActiveSounds.TryGetValue(audioRef, out AudioObj audioObj))
        {
            audioObj.PauseSound();
        }
        else
        {
            Debug.LogWarning($"No active sound found for AudioRef: {audioRef}");
        }
    }

    public void PauseAllMusic()
    {
        foreach (AudioObj obj in ActiveSounds.Values)
        {
            obj.PauseSound();
        }
    }

    public void ResumeMusic(AudioRef audioRef, bool pauseAllMusic = false)
    {
        if (pauseAllMusic)
        {
            PauseAllMusic();
        }
        if (ActiveSounds.TryGetValue(audioRef, out AudioObj audioObj))
        {
            audioObj.ResumeSound();
        }
        else
        {
            PlayMusic(audioRef);
        }
    }

    public void ResumeMultiple(AudioRef[] audioRefs, bool pauseAllMusic = false)
    {
        if (pauseAllMusic)
        {
            PauseAllMusic();
        }
        foreach (AudioRef audioRef in audioRefs)
        {
            if (ActiveSounds.TryGetValue(audioRef, out AudioObj audioObj))
            {
                audioObj.ResumeSound();
            }
            else
            {
                PlayMultiple(audioRefs);
            }
        }
    }


    public void StopMusic(AudioRef audioRef)
    {
        if (ActiveSounds.TryGetValue(audioRef, out AudioObj audioObj))
        {
            audioObj.StopSound();
            ActiveSounds.Remove(audioRef);
        }
        else
        {
            Debug.LogWarning($"No active sound found for AudioRef: {audioRef}");
        }
    }

    public void StopAllMusic()
    {
        foreach (AudioObj obj in ActiveSounds.Values)
        {
            obj.StopSound();
        }
        ActiveSounds.Clear();
    }

    private AudioManager GetAudioManager()
    {
        return GameManager.Instance.AudioManager;
    }
}
