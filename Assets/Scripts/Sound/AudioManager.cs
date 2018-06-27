using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    public static AudioManager instance;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.panStereo = sound.stereoPan;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.output;
        }
    }

    #region API
    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s != null)
            s.source.Play();
        else
            Debug.LogWarning("The sound named : '" + soundName + "' was not found.");
    }

    public void Stop(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s != null)
            s.source.Stop();
        else
            Debug.LogWarning("The sound named : '" + soundName + "' was not found.");
    }

    public void StopAll()
    {
        foreach (var sound in sounds)
        {
            sound.source.Stop();
        }
    }
    #endregion

    #region Setters
    public void SetVolume(string soundName, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s != null)
            s.source.volume = volume;
        else
            Debug.LogWarning("The sound named : '" + soundName + "' was not found.");
    }

    public void SetStereoPan(string soundName, float stereoPan)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s != null)
            s.source.panStereo = stereoPan;
        else
            Debug.LogWarning("The sound named : '" + soundName + "' was not found.");
    }
    #endregion

}
