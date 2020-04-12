using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GuardController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float rotateVelocity = 100.0f;

    Rigidbody rigidbody;
    float verticalInput = 0.0f;
    float horizontalInput = 0.0f;

    public CinemachineFreeLook freeLook;
    Vector3 moveDir = Vector3.zero;
    float moveAmount = 0.0f;

    Guard guard;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        guard = GetComponent<Guard>();
    }

    void Update()
    {
        if(guard.disabled && guard.beingControlled)
            GetInput();
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void HandleRotation()
    {
        moveDir = freeLook.transform.forward * verticalInput;
        moveDir += freeLook.transform.right * horizontalInput;
        moveDir.Normalize();

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        Vector3 targetDir = moveDir;

        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * moveAmount * rotateVelocity);
        transform.rotation = targetRotation;
    }

    void HandleMovement()
    {
        Vector3 v = transform.forward;

        v *= walkSpeed * moveAmount;

        v.y = rigidbody.velocity.y;
        rigidbody.velocity = v;

    }
}