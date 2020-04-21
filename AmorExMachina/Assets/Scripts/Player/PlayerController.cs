using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerSpottedObserver
{
    public float walkSpeed = 10.0f;
    public float sneakSpeed = 2.5f;
    public float rotateVelocity = 100.0f;

    Rigidbody rb;
    float verticalInput = 0.0f;
    float horizontalInput = 0.0f;

    Vector3 moveDir = Vector3.zero;
    float moveAmount = 0.0f;
    bool sneaking = false;
    bool controlling = false;
    
    Guard disabledGuard;
    
    public PlayerSoundSubject playerSoundSubject;
    public GuardHackedSubject guardHackedSubject;
    public PlayerVariables playerVariables;
    public PlayerCamerasVariables cameraVariables;
    public PlayerSpottedSubject playerSpottedSubject;
    public InteractionButtonSubject interactionButtonSubject;

    AudioManager audioManager;

    float accumulateDistance = 0.0f;
    float stepDistance = 0.2f;



    void Awake()
    {
        playerVariables.playerTransform = transform;
        playerVariables.caught = false;
        rb = GetComponent<Rigidbody>();
        cameraVariables.firstPersonCameraFollowTarget = transform.GetChild(1);
        cameraVariables.thirdPersonCameraFollowTarget = transform.GetChild(2);
        playerSpottedSubject.AddObserver(this);
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        PlayerIsKinematicCheck();

        if (GameHandler.currentState != GameState.HACKING && GameHandler.currentState != GameState.NORMALGAME) { return; }

        HackingGuardCheck();

        if (playerVariables.caught)
        {
            GameHandler.currentState = GameState.LOST;
        }

        GetInput();
        
    }

    void FixedUpdate()
    {
        if (GameHandler.currentState != GameState.HACKING && GameHandler.currentState != GameState.NORMALGAME) { return; }

        if (!cameraVariables.switchedCameraToFirstPerson && (verticalInput != 0.0f || horizontalInput != 0.0f))
        {
            //PlaySound();
            HandleRotation();
        }
        else if (cameraVariables.switchedCameraToFirstPerson && (verticalInput != 0.0f || horizontalInput != 0.0f))
        {
            FPSRotate();
        }
        HandleMovement();
    }

    void PlaySound()
    {
        accumulateDistance += Time.deltaTime;
        if (rb.velocity.sqrMagnitude > 0.0f)
        {
            if (accumulateDistance > stepDistance)
            {
                audioManager.Play("Movement");
                accumulateDistance = 0.0f;
            }
        }
        else
        {
            accumulateDistance = 0.0f;
        }
    }

    void PlayerIsKinematicCheck()
    {
        if (GameHandler.currentState != GameState.NORMALGAME)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    void HackingGuardCheck()
    {
        if (disabledGuard != null && Input.GetButtonDown("Square") && !controlling)
        {
            disabledGuard.beingControlled = true;
            controlling = true;
            guardHackedSubject.GuardHackedNotify(disabledGuard.name);
            GameHandler.currentState = GameState.HACKING;
        }
        else if (disabledGuard != null && Input.GetButtonDown("Square") && controlling)
        {
            disabledGuard.beingControlled = false;
            controlling = false;
            guardHackedSubject.GuardHackedNotify("");
            GameHandler.currentState = GameState.NORMALGAME;
        }
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Sneaking"))
        {
            sneaking = !sneaking;
        }
    }

    void FPSRotate()
    {
        if(cameraVariables.switchedCameraToFirstPerson)
        {
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            Quaternion targetRotation = Quaternion.Euler(0.0f, cameraVariables.firstPersonCameraTransform.eulerAngles.y , 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
        }
    }

    void HandleRotation()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        Quaternion targetRotation = Quaternion.Euler(0.0f, cameraVariables.thirdPersonCameraTransform.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
    }

    void HandleMovement()
    {
        //sneaking = Input.GetButton("Sneaking");
        Vector3 v = transform.forward;
        if (cameraVariables.switchedCameraToFirstPerson)
        {
            Vector3 dir = transform.right * horizontalInput + transform.forward * verticalInput;
            v = dir;
        }
        else
        {
            Vector3 dir = cameraVariables.thirdPersonCameraTransform.transform.right * horizontalInput + cameraVariables.thirdPersonCameraTransform.forward * verticalInput;
            v = dir;
        }

        v *= ((sneaking) ? sneakSpeed : walkSpeed) * moveAmount;

        v.y = rb.velocity.y;
        rb.velocity = v;

        if (!sneaking && (verticalInput != 0 || horizontalInput != 0))
            playerSoundSubject.NotifyObservers(SoundType.WALKING, transform.position);

    }

    private void OnTriggerStay(Collider other)
    {
        if (other is CapsuleCollider && other.CompareTag("Guard"))
        {
            Vector3 targetToPlayerDirection = transform.position - other.transform.position;
            float angleToTarget = Vector3.Angle(other.transform.forward, targetToPlayerDirection);
            if (angleToTarget < 180.0f && angleToTarget > 110.0f /*&& Input.GetButtonDown("X")*/)
            {
                interactionButtonSubject.NotifyToShowInteractionButton(InteractionButtons.CROSS);
                if (Input.GetButtonDown("X"))
                {
                    disabledGuard = other.GetComponent<Guard>();
                    if (disabledGuard != null)
                    {
                        disabledGuard.disabled = true;
                    }
                    if (Vector3.Distance(disabledGuard.transform.position, transform.position) >= 50.0f)
                    {
                        disabledGuard = null;
                    }
                }
            }
            else
            {
                interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other is CapsuleCollider && other.CompareTag("Guard"))
        {
            interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
        }
    }

    // Player Spotted Notify.
    public void Notify(Vector3 position)
    {
        if (disabledGuard != null)
        {
            disabledGuard.beingControlled = false;
            disabledGuard = null;
            controlling = false;
            GameHandler.currentState = GameState.NORMALGAME;
        }
    }
}