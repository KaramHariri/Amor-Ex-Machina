using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] PlayerSpottedSubject playerSpottedSubject;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Vector3 position = new Vector3(transform.position.x, 1.0f, transform.position.z);
            playerSpottedSubject.NotifyObservers(position);
        }
    }
}
