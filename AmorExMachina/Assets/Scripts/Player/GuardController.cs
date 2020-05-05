using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float rotateVelocity = 100.0f;
    private float verticalInput = 0.0f;
    private float horizontalInput = 0.0f;
    private float moveAmount = 0.0f;

    private Rigidbody rb = null;
    private Camera mainCamera = null;
    private Guard guard = null;
    [SerializeField] PlayerSoundSubject playerSoundSubject = null;
    [SerializeField] Settings settings = null;

    private AudioManager audioManager = null;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        guard = GetComponent<Guard>();
        audioManager = FindObjectOfType<AudioManager>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(guard.hacked)
        {
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
        }
        if (guard.disabled && guard.hacked)
        {
            GetInput();
        }
    }

    void FixedUpdate()
    {
        if (guard.disabled && guard.hacked)
        {
            HandleRotation();
            HandleMovement();
        }
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if(Input.GetKeyDown(settings.distractGuardWhileHackingController) || Input.GetKeyDown(settings.distractGuardWhileHackingKeyboard))
        {
            playerSoundSubject.NotifyObservers(SoundType.DISTRACTION, this.transform.position);
            audioManager.Play("DistractGuard", this.transform.position);
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