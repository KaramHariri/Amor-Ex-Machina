using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerSpottedObserver
{
    [Range(1.3f, 2.5f)] [SerializeField] private float disableDistance = 1.8f;
    [Range(1.0f, 6.0f)] [SerializeField] private float sneakSpeed = 4.0f;
    [Range(5.0f, 10.0f)] [SerializeField] private float walkSpeed = 7.5f;
    //[Range(50.0f, 150.0f)] [SerializeField] private float rotateVelocity = 100.0f;
    private float verticalInput = 0.0f;
    private float horizontalInput = 0.0f;
    private float moveAmount = 0.0f;
    private bool sneaking = false;
    private bool controlling = false;
    
    private Rigidbody rb = null;
    private Guard disabledGuard = null;
    [SerializeField] List<Guard> possibleGuardsToDisable = null;

    #region Observer pattern subjects
    public PlayerSoundSubject playerSoundSubject = null;
    public GuardHackedSubject guardHackedSubject = null;
    public PlayerVariables playerVariables = null;
    public PlayerCamerasVariables cameraVariables = null;
    public PlayerSpottedSubject playerSpottedSubject = null;
    public InteractionButtonSubject interactionButtonSubject = null;
    #endregion

    private GameObject minimapIcon = null;
    private AudioManager audioManager = null;
    [SerializeField] private Settings settings = null;

    float accumulateDistance = 0.0f;
    float stepDistance = 0.2f;

    void Awake()
    {
        possibleGuardsToDisable = new List<Guard>();
        playerVariables.playerTransform = transform;
        playerVariables.caught = false;
        playerVariables.canHackGuard = true;
        rb = GetComponent<Rigidbody>();
        cameraVariables.firstPersonCameraFollowTarget = transform.GetChild(1);
        cameraVariables.thirdPersonCameraFollowTarget = transform.GetChild(2);
        playerSpottedSubject.AddObserver(this);
        audioManager = FindObjectOfType<AudioManager>();
        Cursor.visible = false;
        rb.isKinematic = false;
        minimapIcon = transform.GetChild(transform.childCount - 1).gameObject;
    }

    void Update()
    {
        PlayerIsKinematicCheck();

        if (GameHandler.currentState != GameState.HACKING && GameHandler.currentState != GameState.NORMALGAME) { return; }

        MinimapCamera.updateIconSize(minimapIcon.transform);
        HackingGuardCheck();

        if (playerVariables.caught)
        {
            GameHandler.currentState = GameState.LOST;
        }

        GetInput();

        DisableGuardCheck();
    }

    void FixedUpdate()
    {
        if (GameHandler.currentState != GameState.HACKING && GameHandler.currentState != GameState.NORMALGAME) { return; }

        if (!cameraVariables.switchedCameraToFirstPerson && (verticalInput != 0.0f || horizontalInput != 0.0f))
        {
            ThirdPersonRotationHandling();
        }
        else if (cameraVariables.switchedCameraToFirstPerson)
        {
            FirstPersonRotationHandling();
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
        if (playerVariables.canHackGuard)
        {
            if (disabledGuard != null && !controlling && (Input.GetKeyDown(settings.hackGuardController) || Input.GetKeyDown(settings.hackGuardKeyboard)))
            {
                disabledGuard.hacked = true;
                controlling = true;
                guardHackedSubject.GuardHackedNotify(disabledGuard.name);
                audioManager.Play("HackGuard", this.transform.position);
                GameHandler.currentState = GameState.HACKING;
                return;
            }
            else if (disabledGuard != null && controlling && (Input.GetKeyDown(settings.hackGuardKeyboard) || Input.GetKeyDown(settings.hackGuardController)))
            {
                disabledGuard.hacked = false;
                controlling = false;
                guardHackedSubject.GuardHackedNotify("");
                audioManager.Play("HackGuard", this.transform.position);
                GameHandler.currentState = GameState.NORMALGAME;
                disabledGuard = null;
                return;
            }
        }
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(settings.movementToggleKeyboard) || Input.GetKeyDown(settings.movementToggleController))
        {
            sneaking = !sneaking;
        }
    }

    void FirstPersonRotationHandling()
    {
        if(cameraVariables.switchedCameraToFirstPerson)
        {
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            Quaternion targetRotation = Quaternion.Euler(0.0f, cameraVariables.firstPersonCameraTransform.eulerAngles.y , 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
        }
    }

    void ThirdPersonRotationHandling()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        Quaternion targetRotation = Quaternion.Euler(0.0f, cameraVariables.thirdPersonCameraTransform.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
    }

    void HandleMovement()
    {
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
        else
            playerSoundSubject.NotifyObservers(SoundType.CROUCHING, transform.position);

    }

    void DisableGuardCheck()
    {
        // If there is a possible guard to disable, we get the closest one and check if we're in a specific angle and distance to the guard to disable it.
        if (possibleGuardsToDisable.Count > 0)
        {
            Guard closestGuard = GetClosestGuard();
            if (closestGuard != null)
            {
                Vector3 targetToPlayerDirection = transform.position - closestGuard.transform.position;
                float angleToTarget = Vector3.Angle(closestGuard.transform.forward, targetToPlayerDirection);
                if (angleToTarget < 180.0f && angleToTarget > 110.0f && targetToPlayerDirection.magnitude <= disableDistance)
                {
                    if (!closestGuard.disabled)
                    {
                        interactionButtonSubject.NotifyToShowInteractionButton(InteractionButtons.CROSS);
                        if (Input.GetKeyDown(settings.disableGuardKeyboard) || Input.GetKeyDown(settings.disableGuardController))
                        {
                            disabledGuard = closestGuard;
                            if (disabledGuard != null)
                            {
                                audioManager.Play("DisableGuard", this.transform.position);
                                disabledGuard.disabled = true;
                            }
                        }
                    }
                    else
                    {
                        interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
                    }
                }
            }
            else
            {
                interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Add the guard that entered to a list of possible guards to disable.
        if (other.CompareTag("Guard"))
        {
            Guard tempScript = other.GetComponent<Guard>();
            //if(!possibleGuardsToDisable.Contains(tempScript))
            //if(!tempScript.disabled)
                possibleGuardsToDisable.Add(tempScript);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove the guard when he exits from the list of possible guards to disable.
        Guard tempScript = other.GetComponent<Guard>();
        //if (possibleGuardsToDisable.Contains(tempScript))
        //{
        possibleGuardsToDisable.Remove(tempScript);
        interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
        //}
    }

    Guard GetClosestGuard()
    {
        Guard closestGuard = null;
        float closestDistanceSquared = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        for (int i = 0; i < possibleGuardsToDisable.Count; i++)
        {
            //if (!possibleGuardsToDisable[i].disabled)
            //{
                Vector3 directionToTarget = possibleGuardsToDisable[i].transform.position - currentPosition;
                float distanceSquaredToTarget = directionToTarget.sqrMagnitude;
                if (distanceSquaredToTarget < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquaredToTarget;
                    closestGuard = possibleGuardsToDisable[i];
                }
            //}
        }
        return closestGuard;
    }

    // Player got notified that it got spotted.
    public void PlayerSpottedNotify(Vector3 position)
    {
        // If the player got notified that it got spotted, the player gets forced out of hacking and/or can't hack anymore.
        if (disabledGuard != null)
        {
            disabledGuard.hacked = false;
            disabledGuard = null;
            controlling = false;
            guardHackedSubject.GuardHackedNotify("");
            audioManager.Play("HackGuard", this.transform.position);
            GameHandler.currentState = GameState.NORMALGAME;
        }
    }

    void OnDestroy()
    {
        playerSpottedSubject.RemoveObserver(this);
    }
}