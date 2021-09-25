using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] protected Sound[] sounds;

    private void Awake() 
    {
        foreach (Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
            s.Source.playOnAwake = s.PlayOnAwake;
            s.Source.spatialBlend = s.SpatialBlend;
        }
    }

    public void Play(string name, bool force)
    {
        Sound s = GetSound(name);
        if (s == null)
        {
            Debug.LogWarning("Sound \"" + name + "\" not found!");
            return;
        }
        if (!s.Source.isPlaying || force)
            s.Source.Play();
    }

    public void Stop(string name)
    {
        Sound s = GetSound(name);
        if (s == null)
        {
            Debug.LogWarning("Sound \"" + name + "\" not found!");
            return;
        }
        s.Source.Stop();
    }

    public void ChangePitch(string name, float pitch)
    {
        Sound s = GetSound(name);
        if (s == null)
        {
            Debug.LogWarning("Sound \"" + name + "\" not found!");
            return;
        }
        s.Source.pitch = pitch;
    }

    private Sound GetSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        return s;
    }
}
