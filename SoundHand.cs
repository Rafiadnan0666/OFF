using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHand : MonoBehaviour
{
    [Header("Ambient Sound Settings")]
    public List<AudioClip> ambientSounds;
    public float volume = 1f;           

    private List<AudioSource> audioSources = new List<AudioSource>();


    void Start()
    {
      
        foreach (var clip in ambientSounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.loop = true;
            source.Play();
            audioSources.Add(source);
        }
    }
}
