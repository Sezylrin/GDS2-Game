using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioObj : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private AudioSource source;
    private bool isPaused = false;
    private AudioManager manager;
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
    }

    public void PauseSound()
    {
        source.Pause();
        isPaused = true;
    }

    public void ResumeSound()
    {
        source.UnPause();
        isPaused = false;
    }

    public void StopSound()
    {
        source.Stop();
        OnComplete();
    }

    private void OnComplete()
    {
        enabled = false;
        isPaused = false;
        manager.ReAddToStack(this);
    }
}
