using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [SerializeField] string name;

    [SerializeField] AudioClip clip;

    [Range(0, 1)]
    [SerializeField] float volume;

    [Range(0.1f, 3)]
    [SerializeField] float pitch;

    [SerializeField] bool loop;
    [SerializeField] bool playOnAwake;

    [Range(0,1)]
    [SerializeField] float spatialBlend;

    AudioSource source;
    
    public AudioClip Clip { get => clip; }
    public float Volume { get => volume; }
    public float Pitch { get => pitch; }
    public bool Loop { get => loop; }
    public AudioSource Source { get => source; set => source = value; }
    public float SpatialBlend { get => spatialBlend; }
    public string Name { get => name; }
    public bool PlayOnAwake { get => playOnAwake; }
}
