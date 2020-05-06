using System.Collections;
using UnityEngine;
using Cinemachine;

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
    public GuardType guardType = GuardType.MOVING;
    public MovementType movementType = MovementType.WAIT_AFTER_FULL_CYCLE;
    public GuardState guardState = GuardState.NORMAL;

    [HideInInspector] public GuardMovement guardMovement = null;
    [HideInInspector] public GuardSensing sensing = null;
    
    private GuardBehaviourTree guardBehavioralTree = null;
    [HideInInspector] public MeshRenderer meshRenderer = null;
    [HideInInspector] public Color currentColor = Color.white;
    [HideInInspector] public CinemachineVirtualCamera vC;

    /*[HideInInspector]*/ public bool disabled = false;
    [HideInInspector] public bool hacked = false;
    [HideInInspector] public bool assist = false;

    public PlayerSoundSubject playerSoundSubject = null;
    public PlayerSpottedSubject playerSpottedSubject = null;
    public GuardVariables guardVariables = null;
    public PlayerVariables playerVariables = null;

    private GameObject minimapIcon = null;
    [SerializeField] private bool visibleInMiniMap = false;
    private Camera mainCamera = null;

    [SerializeField] LayerMask raycastCheckLayer = 0;

    public void Awake()
    {
        GuardGetComponents();
        InitGuardCameraAndPath();
        AddGuardAsObserver();
        InitMiniMap();
        sensing.GuardSensingAwake();
        guardMovement.GuardMovementAwake();

        currentColor = guardVariables.patrolColor;
        mainCamera = Camera.main;
        raycastCheckLayer = LayerMask.GetMask("Walls");
    }

    public void Start()
    {
        guardBehavioralTree = new GuardBehaviourTree(this);
        StartCoroutine("Run");
    }

    public void Update()
    {
        MinimapCamera.updateIconSize(minimapIcon.transform);
        ActivateMinimapIconCheck();
        UpdateGuardState();

        if(guardState != GuardState.NORMAL) { return; }

        if (sensing.PlayerDetectedCheck())
        {
            playerSpottedSubject.NotifyObservers(playerVariables.playerTransform.position);
        }
    }

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
            guardBehavioralTree.Run();
            yield return null;
        }
    }

    void GuardGetComponents()
    {
        guardMovement = GetComponent<GuardMovement>();
        sensing = GetComponent<GuardSensing>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void InitGuardCameraAndPath()
    {
        int index = transform.GetSiblingIndex();
        guardMovement.pathHolder = GameObject.Find("GuardsPathsHolder").transform.GetChild(index);
        vC = GameObject.Find("GuardsCamerasHolder").transform.GetChild(index).GetComponent<CinemachineVirtualCamera>();
        vC.m_Follow = transform.GetChild(transform.childCount - 1);
    }

    void InitMiniMap()
    {
        minimapIcon = transform.GetChild(0).gameObject;
        minimapIcon.SetActive(false);
        visibleInMiniMap = false;
    }

    void AddGuardAsObserver()
    {
        playerSoundSubject.AddObserver(this);
        playerSpottedSubject.AddObserver(this);
    }

    bool ActivateMinimapIconCheck()
    {
        RaycastHit raycastHit;
        if (!visibleInMiniMap && GuardInCameraFieldOfView())
        {
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            if(!Physics.Raycast(transform.position, directionToCamera.normalized, out raycastHit, directionToCamera.magnitude, raycastCheckLayer))
            {
                minimapIcon.SetActive(true);
                visibleInMiniMap = true;
                return true;
            }
        }
        else if(sensing.PlayerDetectedCheck())
        {
            minimapIcon.SetActive(true);
            visibleInMiniMap = true;
            return true;
        }
        return false;
    }

    bool GuardInCameraFieldOfView()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1;
    }

    public void PlayerSoundNotify(SoundType soundType, Vector3 position)
    {
        if(guardState != GuardState.NORMAL) { return; }

        if (soundType == SoundType.WALKING)
        {
            if (sensing.canHear)
            {
                if (sensing.CalculateLength(playerVariables.playerTransform.position) <= sensing.sensingCollider.radius)
                {
                    sensing.suspicious = true;
                    guardMovement.SetInvestigationPosition(position);
                    guardMovement.ResetIdleTimer();
                }
            }
        }
        else if (soundType == SoundType.DISTRACTION)
        {
            if (sensing.CalculateLength(position) <= sensing.sensingCollider.radius)
            {
                sensing.distracted = true;
                guardMovement.SetInvestigationPosition(position);
                guardMovement.ResetIdleTimer();
            }
        }
        else if(soundType == SoundType.CROUCHING)
        {
            if(sensing.detectionAmount < sensing.maxDetectionAmount)
                sensing.suspicious = false;
        }
    }

    public void PlayerSpottedNotify(Vector3 position)
    {
        if (guardState != GuardState.NORMAL) { return; }

        if (sensing.CalculateLength(position) <= guardVariables.maxBackupRadius)
        {
            assist = true;
            guardMovement.SetAssistPosition(position);
        }
    }

    void OnDestroy()
    {
        playerSoundSubject.RemoveObserver(this);
        playerSpottedSubject.RemoveObserver(this);
    }
}
