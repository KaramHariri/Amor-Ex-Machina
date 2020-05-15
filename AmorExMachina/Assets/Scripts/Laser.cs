using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] PlayerSpottedSubject playerSpottedSubject = null;
    private AudioManager audioManager = null;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            audioManager.Play("Alarm", transform.position);
            Vector3 position = new Vector3(transform.position.x, 1.0f, transform.position.z);
            playerSpottedSubject.NotifyObservers(position);
        }
    }
}