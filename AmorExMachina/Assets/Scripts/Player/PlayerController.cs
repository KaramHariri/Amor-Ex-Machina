using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerSpottedObserver
{
    [Range(1.3f, 2.5f)] [SerializeField] private float disableDistance = 1.8f;
    [Range(1.0f, 6.0f)] [SerializeField] private float sneakSpeed = 4.0f;
    [Range(5.0f, 10.0f)] [SerializeField] private float walkSpeed = 7.5f;
    private float verticalInput = 0.0f;
    private float horizontalInput = 0.0f;
    private float moveAmount = 0.0f;
    private bool sneaking = true;
    [HideInInspector] public bool hacking = false;
    public static bool canHackGuard = true;
    
    private Rigidbody rb = null;
    [HideInInspector] public Guard disabledGuard = null;
    [SerializeField] List<Guard> possibleGuardsToDisable = null;

    [SerializeField] private float hackingTimer = 0.0f;
    [SerializeField] private float maxHackingTimer = 20.0f;

    private GameObject minimapIcon = null;

    [SerializeField] private SkinnedMeshRenderer playerMeshRenderer = null;

    float accumulateDistance = 0.0f;
    [SerializeField] float sneakingStepDistance = 0.75f;
    [SerializeField] float walkingStepDistance = 0.5f;
    private float stepDistanceModifier = 0.0f;

    private PlayerSoundSubject playerSoundSubject = null;
    private GuardHackedSubject guardHackedSubject = null;
    private PlayerCamerasVariables cameraVariables = null;
    private PlayerSpottedSubject playerSpottedSubject = null;
    private InteractionButtonSubject interactionButtonSubject = null;
    private GuardDisabledSubject guardDisabledSubject = null;
    private AudioManager audioManager = null;
    private Settings settings = null;

    public static Transform thirdPersonCameraAim = null;
    public static Transform firstPersonCameraAim = null;

    //Added 2020-05-24
    [SerializeField] LayerMask wallsLayer = 0;

    //Added 2020-05-25
    private SonarActivator sonarActivator;

    void Awake()
    {
        thirdPersonCameraAim = transform.Find("ThirdPersonAim");
        firstPersonCameraAim = transform.Find("FirstPersonAim");
        possibleGuardsToDisable = new List<Guard>();
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        rb.isKinematic = false;
        minimapIcon = gameObject.transform.Find("MinimapIcon").gameObject;

        //Added 2020-05-24
        wallsLayer = LayerMask.GetMask("Walls");

        //Added 2020-05-25
        sonarActivator = FindObjectOfType<SonarActivator>();
    }

    void Start()
    {
        GetStaticReferencesFromGameHandler();
    }

    void GetStaticReferencesFromGameHandler()
    {
        playerSoundSubject = GameHandler.playerSoundSubject;
        if (playerSoundSubject == null)
        {
            Debug.Log("PlayerController can't find PlayerSoundSubject in GameHandler");
        }

        guardHackedSubject = GameHandler.guardHackedSubject;
        if (guardHackedSubject == null)
        {
            Debug.Log("PlayerController can't find GuardHackedSubject in GameHandler");
        }

        cameraVariables = GameHandler.playerCamerasVariables;
        if (cameraVariables == null)
        {
            Debug.Log("PlayerController can't find PlayerCamerasVariables in GameHandler");
        }

        playerSpottedSubject = GameHandler.playerSpottedSubject;
        if (playerSpottedSubject == null)
        {
            Debug.Log("PlayerController can't find PlayerSpottedSubject in GameHandler");
        }
        playerSpottedSubject.AddObserver(this);

        interactionButtonSubject = GameHandler.interactionButtonSubject;
        if (interactionButtonSubject == null)
        {
            Debug.Log("PlayerController can't find InteractionButtonSubject in GameHandler");
        }

        guardDisabledSubject = GameHandler.guardDisabledSubject;
        if (guardDisabledSubject == null)
        {
            Debug.Log("PlayerController can't find GuardDisabledSubject in GameHandler");
        }

        audioManager = GameHandler.audioManager;
        if (audioManager == null)
        {
            Debug.Log("PlayerController can't find AudioManager in GameHandler");
        }

        settings = GameHandler.settings;
        if (settings == null)
        {
            Debug.Log("PlayerController can't find Settings in GameHandler");
        }
    }

    void Update()
    {
        PlayerIsKinematicCheck();

        if (GameHandler.currentState != GameState.HACKING && GameHandler.currentState != GameState.NORMALGAME) { return; }

        MinimapCamera.updateIconSize(minimapIcon.transform);
        HackingGuardCheck();

        if (GameHandler.playerIsCaught)
        {
            GameHandler.currentState = GameState.LOST;
        }

        if (hacking)
        {
            UpdateHackingTimer();
            UIManager.updateTimer(hackingTimer);
        }
        else
        {
            hackingTimer = maxHackingTimer;
        }

        if (cameraVariables.switchedCameraToFirstPerson && !hacking)
        {
            playerMeshRenderer.enabled = false;
        }
        else
        {
            playerMeshRenderer.enabled = true;
        }

        if (sneaking)
        {
            Vector3 newCameraPosition = new Vector3(firstPersonCameraAim.localPosition.x, 1.14f, firstPersonCameraAim.localPosition.z);
            firstPersonCameraAim.localPosition = Vector3.Lerp(firstPersonCameraAim.localPosition, newCameraPosition, Time.deltaTime * 5.0f);
        }
        else
        {
            Vector3 newCameraPosition = new Vector3(firstPersonCameraAim.localPosition.x, 1.65f, firstPersonCameraAim.localPosition.z);
            firstPersonCameraAim.localPosition = Vector3.Lerp(firstPersonCameraAim.localPosition, newCameraPosition, Time.deltaTime * 5.0f);
        }

        GetInput();

        if(!hacking)
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
        accumulateDistance += (Time.deltaTime * stepDistanceModifier);
        if (rb.velocity.sqrMagnitude > 0.0f)
        {
            if (accumulateDistance > sneakingStepDistance && sneaking)
            {
                audioManager.Play("PlayerWalking");
                accumulateDistance = 0.0f;
            }
            else if(accumulateDistance > walkingStepDistance && !sneaking)
            {
                audioManager.Play("PlayerWalking");
                accumulateDistance = 0.0f;
            }
        }
        else
        {
            accumulateDistance = 0.0f;
        }
    }

    void UpdateHackingTimer()
    {
        hackingTimer -= Time.deltaTime;
        if (hackingTimer <= 0.0f)
        {
            hackingTimer = 0.0f;
            UIManager.deactivateTimer();

            disabledGuard.hacked = false;
            //Added 2020-05-21
            disabledGuard.guardMovement.isDisabled = true;
            disabledGuard = null;
            hacking = false;
            guardHackedSubject.GuardHackedNotify("");
            audioManager.Play("HackGuard", this.transform.position);
            GameHandler.currentState = GameState.NORMALGAME;

            //Added 2020-05-25
            if (sonarActivator != null)
            {
                sonarActivator.hidePulse();
            }
        }
    }

    void PlayerIsKinematicCheck()
    {
        if (GameHandler.currentState != GameState.NORMALGAME && GameHandler.currentState != GameState.MENU)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
        }
    }

    void HackingGuardCheck()
    {
        if (disabledGuard == null || disabledGuard.disabled == false) { return; }
        
        if (canHackGuard)
        {
            if (!hacking && (Input.GetKeyDown(settings.hackGuardController) || Input.GetKeyDown(settings.hackGuardKeyboard)))
            {
                disabledGuard.hacked = true;
                hacking = true;            
                //Added 2020-05-21
                disabledGuard.guardMovement.isDisabled = false;
                guardHackedSubject.GuardHackedNotify(disabledGuard.name);
                audioManager.Play("HackGuard", this.transform.position);
                UIManager.activateTimer();
                GameHandler.currentState = GameState.HACKING;
                return;
            }
            else if (hacking && (Input.GetKeyDown(settings.hackGuardKeyboard) || Input.GetKeyDown(settings.hackGuardController)))
            {
                disabledGuard.hacked = false;
                hacking = false;
                //Added 2020-05-21
                disabledGuard.guardMovement.isDisabled = true;
                guardHackedSubject.GuardHackedNotify("");
                audioManager.Play("HackGuard", this.transform.position);
                UIManager.deactivateTimer();
                GameHandler.currentState = GameState.NORMALGAME;
                disabledGuard = null;

                //Added 2020-05-25
                if (sonarActivator != null)
                {
                    sonarActivator.hidePulse();
                }

                return;
            }
        }
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        if(Input.GetKey(settings.movementToggleController) || Input.GetKey(settings.movementToggleKeyboard))
        {
            sneaking = false;
        }
        else
        {
            sneaking = true;
        }
    }

    void FirstPersonRotationHandling()
    {
        if(cameraVariables.switchedCameraToFirstPerson)
        {
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            Quaternion targetRotation = Quaternion.Euler(0.0f, FirstPersonCinemachine.firstPersonCameraTransform.eulerAngles.y , 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
        }
    }

    void ThirdPersonRotationHandling()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        Quaternion targetRotation = Quaternion.Euler(0.0f, ThirdPersonCinemachine.thirdPersonCameraTransform.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
    }

    void HandleMovement()
    {
        if (GameHandler.currentState == GameState.HACKING) { return; }

        Vector3 v = transform.forward;
        if (cameraVariables.switchedCameraToFirstPerson)
        {
            Vector3 dir = transform.right * horizontalInput + transform.forward * verticalInput;
            v = dir;
        }
        else
        {
            Vector3 dir = ThirdPersonCinemachine.thirdPersonCameraTransform.transform.right * horizontalInput 
                        + ThirdPersonCinemachine.thirdPersonCameraTransform.forward * verticalInput;
            v = dir;
        }

        stepDistanceModifier = v.magnitude;

        v.Normalize();
        v *= ((sneaking) ? sneakSpeed : walkSpeed) * moveAmount;

        v.y = rb.velocity.y;
        rb.velocity = v;

        if (!sneaking && (verticalInput != 0 || horizontalInput != 0))
            playerSoundSubject.NotifyObservers(SoundType.WALKING, transform.position);
        else
            playerSoundSubject.NotifyObservers(SoundType.CROUCHING, transform.position);

        PlaySound();
    }

    void DisableGuardCheck()
    {
        if (possibleGuardsToDisable.Count > 0)
        {
            Guard closestGuard = GetClosestGuard();
            if (closestGuard != null)
            {
                Vector3 targetToPlayerDirection = transform.position - closestGuard.transform.position;
                //Added 2020-05-24
                {
                    if(Physics.Raycast(transform.position + new Vector3(0.0f, 1.0f, 0.0f), -targetToPlayerDirection, targetToPlayerDirection.magnitude, wallsLayer))
                    {
                        interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
                        return;
                    }
                }
                float angleToTarget = Vector3.Angle(closestGuard.transform.forward, targetToPlayerDirection);
                if (!closestGuard.sensing.PlayerDetectedCheck() && !closestGuard.sensing.playerWasDetectedCheck() && !closestGuard.sensing.Suspicious() 
                    && !closestGuard.sensing.Alarmed() && targetToPlayerDirection.magnitude <= disableDistance && disabledGuard != closestGuard)
                {
                    //if (!closestGuard.sensing.Suspicious())
                    //{
                        interactionButtonSubject.NotifyToShowInteractionButton(InteractionButtons.CROSS);
                        DisableGuardInputCheck(closestGuard);
                    //}
                    //else if (closestGuard.sensing.Suspicious() || closestGuard.sensing.playerWasDetectedCheck())
                    //{
                    //    if (angleToTarget < 180.0f && angleToTarget > 120.0f)
                    //    {
                    //        interactionButtonSubject.NotifyToShowInteractionButton(InteractionButtons.CROSS);
                    //        DisableGuardInputCheck(closestGuard);
                    //    }
                    //    else
                    //        interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
                    //}
                    //else
                    //{
                    //    interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
                    //}
                }
                else if((closestGuard.sensing.PlayerDetectedCheck() || closestGuard.sensing.playerWasDetectedCheck() || closestGuard.sensing.Suspicious() || closestGuard.sensing.Alarmed())
                        && targetToPlayerDirection.magnitude <= disableDistance && disabledGuard != closestGuard)
                {
                    if (angleToTarget < 180.0f && angleToTarget > 120.0f)
                    {
                        interactionButtonSubject.NotifyToShowInteractionButton(InteractionButtons.CROSS);
                        DisableGuardInputCheck(closestGuard);
                    }
                    else
                        interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
                }
                else
                {
                    interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
                }
            }
            else
            {
                interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
            }
        }
    }

    void DisableGuardInputCheck(Guard guard)
    {
        if (Input.GetKeyDown(settings.disableGuardKeyboard) || Input.GetKeyDown(settings.disableGuardController))
        {
            disabledGuard = guard;
            if (disabledGuard != null)
            {
                // Added 20-05-13.
                disabledGuard.PlayDisableVFX();
                /////

                guardDisabledSubject.GuardDisabledNotify(disabledGuard);
                audioManager.Play("DisableGuard", this.transform.position);
                disabledGuard.disabled = true;
                //Added 2020-05-21
                disabledGuard.guardMovement.isDisabled = true;

            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Add the guard that entered to a list of possible guards to disable.
        if (other.CompareTag("Guard"))
        {
            Guard tempScript = other.GetComponent<Guard>();
            possibleGuardsToDisable.Add(tempScript);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove the guard when he exits from the list of possible guards to disable.
        Guard tempScript = other.GetComponent<Guard>();
        possibleGuardsToDisable.Remove(tempScript);
        interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CROSS);
    }

    Guard GetClosestGuard()
    {
        Guard closestGuard = null;
        float closestDistanceSquared = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        for (int i = 0; i < possibleGuardsToDisable.Count; i++)
        {
            Vector3 directionToTarget = possibleGuardsToDisable[i].transform.position - currentPosition;
            float distanceSquaredToTarget = directionToTarget.sqrMagnitude;
            if (distanceSquaredToTarget < closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquaredToTarget;
                closestGuard = possibleGuardsToDisable[i];
            }
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
            //Added 2020-05-21
            disabledGuard.guardMovement.isDisabled = true;
            disabledGuard = null;
            hacking = false;
            guardHackedSubject.GuardHackedNotify("");
            //Added 2020-05-20
            UIManager.deactivateTimer();

            if (hacking)
                audioManager.Play("HackGuard", this.transform.position);
            GameHandler.currentState = GameState.NORMALGAME;
        }
    }

    void OnDestroy()
    {
        playerSpottedSubject.RemoveObserver(this);
    }
}