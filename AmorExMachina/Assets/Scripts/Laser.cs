using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //[SerializeField] PlayerSpottedSubject playerSpottedSubject = null;
    //private AudioManager audioManager = null;

    private PlayerSpottedSubject playerSpottedSubject = null;
    private AudioManager audioManager = null;

    private void Start()
    {
        playerSpottedSubject = GameHandler.playerSpottedSubject;
        if(playerSpottedSubject == null)
        {
            Debug.Log("Laser can't find PlayerSpottedSubject in GameHandler");
        }

        audioManager = GameHandler.audioManager;
        if(audioManager == null)
        {
            Debug.Log("Laser can't find AudioManager in GameHandler");
        }
        //audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            audioManager.Play("Alarm", transform.position);
            playerSpottedSubject.NotifyObservers(transform.position);
        }
    }
}