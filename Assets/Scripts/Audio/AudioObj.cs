using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioObj : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private AudioSource source;
    private bool isPaused = false;
    private AudioManager manager;
    private float initialVolume;
    void Start()
    {
    }
    public void Init(AudioManager manager)
    {
        this.manager = manager;
    }
    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying && isPaused == false)
        {
            OnComplete();
        }
    }

    public void StartPlaying(AudioClip clipToPlay, AudioMixerGroup group, bool loop, float volume)
    {
        enabled = true;
        source.outputAudioMixerGroup = group;
        source.clip = clipToPlay;
        source.Play();
        source.loop = loop;
        source.volume = volume;
        initialVolume = volume;
    }

    public void PauseSound(bool fade = false, float dur = 1)
    {
        if (!fade)
        {
            source.Pause();
            isPaused = true;
        }
        else
        {
            source.DOFade(0, dur).OnComplete(() =>
             {
                 source.Pause();
                 isPaused = true;
             });
        }
    }

    public void ResumeSound(bool fade = false, float dur = 1)
    {
        if (!fade) 
            source.volume = initialVolume;
        source.UnPause();
        isPaused = false;
        if (fade)
        {
            source.DOFade(initialVolume, dur);
        }
        
    }

    public void StopSound(bool fade = false, float dur = 1)
    {
        if (!fade)
        {
            source.Stop();
            OnComplete();
        }
        else
        {
            source.DOFade(0, dur).OnComplete(() =>
            {
                source.Stop();
                OnComplete();
            });
        }
    }

    private void OnComplete()
    {
        enabled = false;
        isPaused = false;
        manager.ReAddToStack(this);
    }

    public void FadeIn(float startVol, float endVol, float dur)
    {
        source.volume = startVol;
        source.DOFade(endVol, dur);
    }

}
