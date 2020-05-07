using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Audio
{
    public enum AudioType
    {
        EFFECT,
        FOOTSTEPS,
        VOICE,
        MUSIC
    }

    public string name;
    public AudioType audioType;
    public AudioClip[] clip;

    [HideInInspector] public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;
    [Range(2f, 1000f)]
    public float maxSoundDistance = 200f;
    public bool loop = false;
    public bool playOnAwake = false;
    public bool threeDimensionalSound = true;
    [HideInInspector]
    public AudioSource aS;
}

public class AudioManager : MonoBehaviour
{
    public Audio[] soundFX;
    [SerializeField] private Settings settings = null;

    private Audio generalAmbience = null;
    private Audio guardsChasingPlayer = null;
    private Audio gameOver = null;
    private float musicVolume = 0.0f;

    void Awake()
    {
        //settings.masterVolume = 0.5f;
        foreach (Audio aud in soundFX)
        {
            aud.aS = gameObject.AddComponent<AudioSource>();
            SetVolumeFromSettings(aud);
            //aud.aS.clip = aud.clip;
            aud.aS.volume = aud.volume;
            aud.aS.loop = aud.loop;
            aud.aS.pitch = aud.pitch;
            aud.aS.playOnAwake = aud.playOnAwake;
            aud.aS.maxDistance = aud.maxSoundDistance;

            if (aud.threeDimensionalSound)
            {
                aud.aS.spatialBlend = 1.0f;
            }
            else
                aud.aS.spatialBlend = 0.0f;

            InitBackgroundMusic(aud);
        }
    }

    public void Play(string name, Vector3 position = default)
    {
        Audio aud = Array.Find(soundFX, Audio => Audio.name == name);
        if (aud == null)
        {
            Debug.Log("Audio : " + aud.name + " not found");
            return;
        }
        aud.aS.clip = aud.clip[UnityEngine.Random.Range(0, aud.clip.Length)];
        SetVolumeFromSettings(aud);
        aud.aS.volume = aud.volume * (1 + UnityEngine.Random.Range(-aud.randomVolume / 2f, aud.randomVolume / 2));
        aud.aS.pitch = aud.pitch * (1 + UnityEngine.Random.Range(-aud.randomPitch / 2f, aud.randomPitch / 2));
        if (!aud.aS.isPlaying)
        {
            if (aud.aS.spatialBlend == 1)
            {
                aud.aS.maxDistance = aud.maxSoundDistance;
                PlayAudioSource(name, aud.aS.clip, position, aud.aS.volume, aud.aS.pitch, aud.aS.maxDistance, aud.aS.loop);
            }
            else
                aud.aS.Play();
        }
    }

    public void UpdateBackGroundMusic(PlayerState playerState)
    {
        musicVolume = settings.musicVolume * settings.masterVolume;
        switch (playerState)
        {
            case PlayerState.SPOTTED:
                guardsChasingPlayer.aS.volume = Mathf.Lerp(guardsChasingPlayer.aS.volume, musicVolume, Time.deltaTime * 2.0f);
                generalAmbience.aS.volume = Mathf.Lerp(generalAmbience.aS.volume, 0.0f, Time.deltaTime * 2.0f);
                break;
            case PlayerState.NOTSPOTTED:
                generalAmbience.aS.volume = Mathf.Lerp(generalAmbience.aS.volume, musicVolume, Time.deltaTime * 2.0f);
                guardsChasingPlayer.aS.volume = Mathf.Lerp(guardsChasingPlayer.aS.volume, 0.0f, Time.deltaTime * 2.0f);
                break;
            case PlayerState.CAUGHT:
                gameOver.aS.volume = Mathf.Lerp(gameOver.aS.volume, musicVolume, Time.deltaTime * 2.0f);
                guardsChasingPlayer.aS.volume = Mathf.Lerp(guardsChasingPlayer.aS.volume, 0.0f, Time.deltaTime * 2.0f);
                generalAmbience.aS.volume = Mathf.Lerp(generalAmbience.aS.volume, 0.0f, Time.deltaTime * 2.0f);
                break;
            default:
                break;
        }
    }

    public void Mute(string name)
    {
        StartCoroutine(MuteSound(name));
    }

    IEnumerator MuteSound(string name)
    {
        Audio aud = Array.Find(soundFX, Audio => Audio.name == name);
        if (aud == null)
        {
            Debug.Log("Audio : " + name + " not found");
            yield break;
        }
        float totalFadingTime = 0.5f;
        float currentFadingTime = 0;
        while (aud.aS.volume > 0)
        {
            currentFadingTime += Time.deltaTime;
            aud.aS.volume = Mathf.Lerp(1, 0, currentFadingTime / totalFadingTime);
            yield return null;
        }
        if (aud.aS.volume <= 0.01f)
        {
            Stop(name);
        }
    }

    public void Stop(string name)
    {
        Audio aud = Array.Find(soundFX, Audio => Audio.name == name);
        if (aud == null)
        {
            Debug.Log("Audio : " + name + " not found");
            return;
        }
        if (aud.aS.isPlaying)
        {
            aud.aS.Stop();
        }

    }

    void PlayAudioSource(string name, AudioClip audioClip, Vector3 position, float volume, float pitch, float maxDistance, bool loop)
    {
        AudioSource audioSource = AudioSourcePool.instance.GetAudioSource();
        audioSource.clip = audioClip;
        audioSource.spatialBlend = 1.0f;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.maxDistance = maxDistance;
        audioSource.gameObject.transform.position = position;
        audioSource.loop = loop;
        audioSource.Play();
    }

    void SetVolumeFromSettings(Audio audio)
    {
        switch(audio.audioType)
        {
            case Audio.AudioType.EFFECT:
                audio.volume = settings.effectsVolume * settings.masterVolume;
                break;
            case Audio.AudioType.FOOTSTEPS:
                audio.volume = settings.footstepsVolume * settings.masterVolume;
                break;
            case Audio.AudioType.VOICE:
                audio.volume = settings.voiceVolume * settings.masterVolume;
                break;
            case Audio.AudioType.MUSIC:
                audio.volume = settings.musicVolume * settings.masterVolume;
                break;
        }
    }

    void InitBackgroundMusic(Audio audio)
    {
        if(audio.audioType == Audio.AudioType.MUSIC)
        {
            switch (audio.name)
            {
                case "GeneralAmbience":
                    generalAmbience = audio;
                    generalAmbience.aS.clip = generalAmbience.clip[0];
                    generalAmbience.aS.Play();
                    break;
                case "GuardsChasingPlayer":
                    guardsChasingPlayer = audio;
                    guardsChasingPlayer.aS.clip = guardsChasingPlayer.clip[0];
                    guardsChasingPlayer.aS.volume = 0.0f;
                    guardsChasingPlayer.aS.Play();
                    break;
                case "GameOver":
                    gameOver = audio;
                    gameOver.aS.clip = gameOver.clip[0];
                    gameOver.aS.volume = 0.0f;
                    gameOver.aS.Play();
                    break;
                default:
                    break;
            }
            musicVolume = settings.musicVolume;
        }
    }
}
