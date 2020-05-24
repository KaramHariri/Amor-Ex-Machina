using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private PlayerSpottedSubject playerSpottedSubject = null;
    private PlayerSoundSubject playerSoundSubject = null;
    private AudioManager audioManager = null;
    private Material material = null;
    private float timer = 0.0f;
    [SerializeField] private float maxPulseTimer = 5.0f;
    [SerializeField] private float pulseFrequency = 15f;

    private void Start()
    {
        playerSpottedSubject = GameHandler.playerSpottedSubject;
        if (playerSpottedSubject == null)
        {
            Debug.Log("Laser can't find PlayerSpottedSubject in GameHandler");
        }

        playerSoundSubject = GameHandler.playerSoundSubject;
        if(playerSoundSubject == null)
        {
            Debug.Log("Laser can't find PlayerSoundSubject in GameHandler");
        }

        audioManager = GameHandler.audioManager;
        if (audioManager == null)
        {
            Debug.Log("Laser can't find AudioManager in GameHandler");
        }

        material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            material.SetFloat("_HexEdgeTimeScale", pulseFrequency);
            material.SetFloat("_HexEdgePosScale", 50.0f);
        }
        else
        {
            timer = 0.0f;
            material.SetFloat("_HexEdgeTimeScale", 5.0f);
            material.SetFloat("_HexEdgePosScale", 50.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer = maxPulseTimer;
            audioManager.Play("Alarm", transform.position);
            //playerSpottedSubject.NotifyObservers(transform.position);
            playerSoundSubject.NotifyObservers(SoundType.ALARM, transform.position);
        }
    }
}