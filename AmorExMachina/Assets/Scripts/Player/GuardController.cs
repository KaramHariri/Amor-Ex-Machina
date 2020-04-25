using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float rotateVelocity = 100.0f;

    Rigidbody rb;
    float verticalInput = 0.0f;
    float horizontalInput = 0.0f;

    Vector3 moveDir = Vector3.zero;

    float moveAmount = 0.0f;

    Camera mainCamera;
    Guard guard;
    [SerializeField]
    PlayerSoundSubject playerSoundSubject;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        guard = GetComponent<Guard>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(guard.beingControlled)
        {
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
        }
        if (guard.disabled && guard.beingControlled)
        {
            GetInput();
            HandleRotation();
        }
    }

    void FixedUpdate()
    {
        if (guard.disabled && guard.beingControlled)
        {
            HandleMovement();
        }
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if(Input.GetButtonDown("X"))
        {
            playerSoundSubject.NotifyObservers(SoundType.DISTRACTION, this.transform.position);
        }
    }

    void HandleRotation()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        Quaternion targetRotation = Quaternion.Euler(0.0f, mainCamera.transform.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
    }

    void HandleMovement()
    {
        Vector3 v = transform.forward;
        Vector3 dir = transform.right * horizontalInput + transform.forward * verticalInput;
        v = dir;

        v *= walkSpeed * moveAmount;

        v.y = rb.velocity.y;
        rb.velocity = v;
    }
}