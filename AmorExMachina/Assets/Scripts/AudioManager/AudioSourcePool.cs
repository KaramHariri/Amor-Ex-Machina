using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSourcePrefab = null;
    [SerializeField]
    private int numberOfAudioSources = 10;

    public static AudioSourcePool instance = null;
    public List<AudioSource> pooledAudioSources = null;
    private Transform holder = null;

    private void Awake()
    {
        instance = this;
        holder = this.transform;
    }

    public void InstantiateAudioPool()
    {
        pooledAudioSources = new List<AudioSource>();
        for (int i = 0; i < numberOfAudioSources; i++)
        {
            AudioSource audioSource = Instantiate(audioSourcePrefab, holder);
            audioSource.playOnAwake = false;
            pooledAudioSources.Add(audioSource);
        }
    }

    //void Start()
    //{
    //    pooledAudioSources = new List<AudioSource>();
    //    for (int i = 0; i < numberOfAudioSources; i++)
    //    {
    //        AudioSource audioSource = Instantiate(audioSourcePrefab, holder);
    //        audioSource.playOnAwake = false;
    //        pooledAudioSources.Add(audioSource);
    //    }
    //}

    public AudioSource GetAudioSource()
    {
        for (int i = 0; i < pooledAudioSources.Count; i++)
        {
            if (!pooledAudioSources[i].isPlaying)
            {
                return pooledAudioSources[i];
            }
        }

        AudioSource audioSource = Instantiate(audioSourcePrefab, holder);
        pooledAudioSources.Add(audioSource);
        return audioSource;
    }
}
