using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementTest : MonoBehaviour
{

    public Transform transform1;
    public float speed = 0.1f;
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 directionToTransform = transform1.position - transform.position;
        Quaternion targetQuaternion = Quaternion.LookRotation(directionToTransform);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, speed * Time.deltaTime);
        transform.position = transform.position + transform.forward * Time.deltaTime * 2.0f; 
    }
}
