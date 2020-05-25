using System.Collections;
using UnityEngine;
using Cinemachine;
using JetBrains.Annotations;

public enum GuardType
{
    STATIONARY,
    MOVING
}

public enum MovementType
{
    WAIT_AT_WAYPOINT,
    WAIT_AFTER_FULL_CYCLE,
    DONT_WAIT
}

public enum GuardState
{
    DISABLED,
    HACKED,
    NORMAL
}

public class Guard : MonoBehaviour, IPlayerSoundObserver, IPlayerSpottedObserver
{
    [Header("Guard Movement Type")]
    public GuardType guardType = GuardType.MOVING;
    public MovementType movementType = MovementType.WAIT_AFTER_FULL_CYCLE;

    [Header("Speed")]
    public float patrolRotationSpeed = 2.5f;
    public float suspiciousRotationSpeed = 3.0f;
    public float chaseRotationSpeed = 3.5f;
    public float patrolSpeed = 2.0f;
    public float suspiciousSpeed = 3.5f;
    public float chaseSpeed = 5.0f;

    [Header("Sensing")]
    public float fieldOfViewRadius = 20.0f;
    public float fieldOfViewAngle = 70.0f;
    public float maxAssistRadius = 10.0f;
    public float minHearingRadius = 5.0f;
    public float maxHearingRadius = 10.0f;
    public float maxDistractionRadius = 15.0f;
    public float alertedHearingRadius = 15.0f;
    public float alarmRadius = 15.0f;

    [Header("Looking Around")]
    public float lookingAroundFrequency = 1.0f;
    public float lookingAroundAngle = 45.0f;

    [Header("Distance based values")]
    public float maxDistanceValue = 0.6f;
    public float minDistanceValue = 2.0f;

    [Header("Timers")]
    public float maxPatrolIdleTimer = 5.0f;
    public float maxLookingAroundTimer = 5.0f;
    public float maxDetectionBarAmount = 2.0f;
    public float maxTimerSincePlayerWasSpotted = 5.0f;

    [Header("Sensing Gizmos")]
    public bool showHearingRadius = true;
    public bool showFieldOfView = true;

    [HideInInspector] public float hearingRadius = 20.0f;

    #region GuardScriptsReferences
    [HideInInspector] public GuardMovement guardMovement = null;
    [HideInInspector] public GuardSensing sensing = null;
    #endregion

    [HideInInspector] public GuardState guardState = GuardState.NORMAL;
    private GuardBehaviourTree guardBehavioralTree = null;
    [HideInInspector] public CinemachineVirtualCamera vC;
    private AudioManager audioManager = null;

    [HideInInspector] public bool disabled = false;
    [HideInInspector] public bool hacked = false;
    [HideInInspector] public bool assist = false;
    //[HideInInspector] public bool updatedRotation = false;

    #region ObserverSubjects
    private PlayerSoundSubject playerSoundSubject = null;
    private PlayerSpottedSubject playerSpottedSubject = null;
    #endregion

    #region Minimap
    private GameObject minimapIcon = null;
    private SpriteRenderer minimapIconSpriteRenderer = null;
    private bool visibleInMiniMap = false;
    private Camera mainCamera = null;
    private LayerMask minimapRaycastCheckLayer = 0;
    #endregion

    private ParticleSystem disableParticleSystem = null;
    private ParticleSystem enableParticleSystem = null;
    
    // Looking around helping vectors.
    //[HideInInspector] public Vector3 lookingAroundPositiveVector = Vector3.zero;
    //[HideInInspector] public Vector3 lookingAroundNegativeVector = Vector3.zero;

    [HideInInspector] public Transform playerTransform = null;
    public Transform guardNeckTransform = null;

    public void Awake()
    {
        GuardGetComponents();
        InitGuardCameraAndPath();
        InitMiniMap();
        guardMovement.GuardMovementInit();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
        minimapRaycastCheckLayer = LayerMask.GetMask("Walls");
        disableParticleSystem = transform.Find("VFX").Find("Guard Disable 1.0 Variant").GetComponent<ParticleSystem>();
        enableParticleSystem = transform.Find("VFX").Find("Guard Reactivated").GetComponent<ParticleSystem>();
    }

    public void Start()
    {
        GetStaticReferencesFromGameHandler();
        AddGuardAsObserver();
        guardBehavioralTree = new GuardBehaviourTree(this);
        StartCoroutine("Run");
    }

    void OnDrawGizmos()
    {
        if (showHearingRadius)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, hearingRadius);
        }

        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfViewAngle, transform.up) * transform.forward * fieldOfViewRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfViewAngle, transform.up) * transform.forward * fieldOfViewRadius;

        if (showFieldOfView)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2);
        }
    }

    public void Update()
    {
        MinimapCamera.updateIconSize(minimapIcon.transform);
        ActivateMinimapIcon();
        UpdateMinimapIcon();
        UpdateGuardState();

        if(guardState != GuardState.NORMAL) { return; }

        if (sensing.PlayerDetectedCheck())
        {
            playerSpottedSubject.NotifyObservers(playerTransform.position);
        }
    }

    //void LateUpdate()
    //{
    //    RotateGuardNeck();
    //}

    //void RotateGuardNeck()
    //{
    //    lookingAroundPositiveVector = new Vector3(0.0f, lookingAroundAngle, 0.0f);
    //    lookingAroundNegativeVector = new Vector3(0.0f, -lookingAroundAngle, 0.0f);
    //    Quaternion from = Quaternion.Euler(lookingAroundPositiveVector);
    //    Quaternion to = Quaternion.Euler(lookingAroundNegativeVector);

    //    float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * lookingAroundFrequency));
    //    guardNeckTransform.localRotation = Quaternion.Lerp(from, to, lerp);
    //}

    void UpdateGuardState()
    {
        if(hacked)
        {
            guardState = GuardState.HACKED;
            return;
        }
        else if(disabled)
        {
            guardState = GuardState.DISABLED;
            return;
        }
        else
        {
            guardState = GuardState.NORMAL;
            return;
        }
    }

    IEnumerator Run()
    {
        while (true)
        {
            if (GameHandler.currentState != GameState.MENU)
            {
                guardMovement.navMeshAgent.enabled = true;
                guardBehavioralTree.Run();
            }
            else
            {
                guardMovement.navMeshAgent.enabled = false;
            }
            yield return null;
        }
    }

    void GetStaticReferencesFromGameHandler()
    {
        playerSoundSubject = GameHandler.playerSoundSubject;
        if (playerSoundSubject == null)
        {
            Debug.Log("Guard can't find PlayerSoundSubject in GameHandler");
        }

        playerSpottedSubject = GameHandler.playerSpottedSubject;
        if (playerSpottedSubject == null)
        {
            Debug.Log("Guard can't find PlayerSpottedSubject in GameHandler");
        }

        audioManager = GameHandler.audioManager;
        if (audioManager == null)
        {
            Debug.Log("Guard can't find AudioManager in GameHandler");
        }
    }

    void GuardGetComponents()
    {
        guardMovement = GetComponent<GuardMovement>();
        sensing = GetComponent<GuardSensing>();
    }

    void InitGuardCameraAndPath()
    {
        int index = transform.GetSiblingIndex();
        guardMovement.pathHolder = GameObject.Find("GuardsPathsHolder").transform.GetChild(index);
        vC = GameObject.Find("GuardsCamerasHolder").transform.GetChild(index).GetComponent<CinemachineVirtualCamera>();
        vC.m_Follow = transform.Find("Aim");
    }

    void InitMiniMap()
    {
        minimapIcon = transform.Find("MinimapIcon").gameObject;
        minimapIconSpriteRenderer = minimapIcon.GetComponent<SpriteRenderer>();
        minimapIcon.SetActive(false);
        visibleInMiniMap = false;
    }

    void AddGuardAsObserver()
    {
        playerSoundSubject.AddObserver(this);
        playerSpottedSubject.AddObserver(this);
    }

    // Updating the minimap icon depending on the guard state.
    void UpdateMinimapIcon()
    {
        if(hacked)
        {
            minimapIconSpriteRenderer.color = Color.Lerp(minimapIconSpriteRenderer.color, Color.white, Time.deltaTime);
        }
        else if(disabled)
        {
            minimapIconSpriteRenderer.color = Color.Lerp(minimapIconSpriteRenderer.color, Color.black, Time.deltaTime);
        }
        else
        {
            minimapIconSpriteRenderer.color = Color.Lerp(minimapIconSpriteRenderer.color, Color.red, Time.deltaTime);
        }
    }
    
    // Activate minimap icon if the guard is visible to the camera and there is no walls in between.
    void ActivateMinimapIcon()
    {
        if(visibleInMiniMap) { return; }
        RaycastHit raycastHit;
        if (GuardInCameraFieldOfView())
        {
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            // Raycast from the guard towards the camera to check if it hits a wall.
            if(!Physics.Raycast(transform.position, directionToCamera.normalized, out raycastHit, directionToCamera.magnitude, minimapRaycastCheckLayer))
            {
                minimapIcon.SetActive(true);
                visibleInMiniMap = true;
                return;
            }
        }
        else if(sensing.PlayerDetectedCheck())
        {
            minimapIcon.SetActive(true);
            visibleInMiniMap = true;
            return;
        }
    }

    // Checking if the guard is in field of view of the camera.
    bool GuardInCameraFieldOfView()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1;
    }

    public void PlayEnablingSound(Vector3 position)
    {
        audioManager.Play("EnableGuard", position);
    }

    // Update looking around vectors depending on the current Y eulerAngle.
    //public void UpdateLookingAroundAngle()
    //{
    //    if (!updatedRotation)
    //    {
    //        lookingAroundPositiveVector = new Vector3(0.0f, lookingAroundAngle, 0.0f);
    //        lookingAroundNegativeVector = new Vector3(0.0f, -lookingAroundAngle, 0.0f);
    //        updatedRotation = true;
    //    }
    //}

    // Play getting disabled particle system.
    public void PlayDisableVFX()
    {
        disableParticleSystem.Play();
    }

    // Play getting enabled particle system.
    public void PlayerEnableVFX()
    {
        enableParticleSystem.Play();
    }

    // Get Sound notification from the player.
    public void PlayerSoundNotify(SoundType soundType, Vector3 position)
    {
        if (guardState != GuardState.NORMAL) { return; }

        if (soundType == SoundType.WALKING)
        {
            if (sensing.canHear)
            {
                if (sensing.CalculateLength(playerTransform.position) <= hearingRadius)
                {
                    sensing.suspicious = true;
                    //updatedRotation = false;
                    guardMovement.SetInvestigationPosition(position);
                }
            }
        }
        else if (soundType == SoundType.ALARM)
        {
            if(sensing.CalculateLength(position) <= alarmRadius)
            {
                sensing.alarmed = true;
                guardMovement.SetAlarmInvestigationPosition(position);
                guardMovement.ResetIdleTimer();
            }
        }
        else if (soundType == SoundType.DISTRACTION)
        {
            if (sensing.CalculateLength(position) <= maxDistractionRadius)
            {
                sensing.distracted = true;
                guardMovement.SetDistractionInvestigationPosition(position);
                guardMovement.ResetIdleTimer();
            }
        }
        else if (soundType == SoundType.CROUCHING)
        {
            if (sensing.detectionAmount < maxDetectionBarAmount)
                sensing.suspicious = false;
        }
    }

    // Get notified about player got spotted.
    public void PlayerSpottedNotify(Vector3 position)
    {
        if (guardState != GuardState.NORMAL) { return; }

        if (sensing.CalculateLength(position) <= maxAssistRadius)
        {
            assist = true;
            guardMovement.SetAssistPosition(position);
        }
    }

    // Destroy observers subjects.
    void OnDestroy()
    {
        playerSoundSubject.RemoveObserver(this);
        playerSpottedSubject.RemoveObserver(this);
    }
}
