using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float inputDelay = 0.1f;
    public float forwardVelocity = 12.0f;
    public float rotateVelocity = 100.0f;

    Quaternion targetRotation;
    Vector3 velocity = Vector3.zero;
    Rigidbody rigidbody;
    float forwardInput, turnInput;

    void Start()
    {
        targetRotation = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();

        forwardInput = turnInput = 0.0f;
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void Update()
    {
        GetInput();
        Turn();
    }

    void FixedUpdate()
    {
        Move();
        rigidbody.velocity = transform.TransformDirection(velocity);
    }

    void Move()
    {
        if (Mathf.Abs(forwardInput) > inputDelay)
        {
            velocity.z = forwardInput * forwardVelocity;
        }
        else
            velocity.z = 0.0f;
    }

    void Turn()
    {
        if (Mathf.Abs(turnInput) > inputDelay)
        {
            targetRotation *= Quaternion.AngleAxis(rotateVelocity * turnInput * Time.deltaTime, Vector3.up);
        }
        transform.rotation = targetRotation;
    }
}