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

public class Guard : MonoBehaviour, IPlayerSoundObserver, IPlayerSpottedObserver
{
    public GuardType guardType = GuardType.MOVING;
    public MovementType movementType = MovementType.WAIT_AFTER_FULL_CYCLE;

    GuardBehaviourTree guardBehavioralTree;
    [HideInInspector]
    public GuardMovement guardMovement;
    [HideInInspector]
    public GuardSensing sensing;
    
    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public Color currentColor;

    [HideInInspector]
    public bool disabled = false;
    [HideInInspector]
    public bool beingControlled = false;
    [HideInInspector]
    public bool assist = false;

    public PlayerSoundSubject playerSoundSubject;
    public PlayerSpottedSubject playerSpottedSubject;
    public GuardVariables guardVariables;
    public PlayerVariables playerVariables;

    public bool showSensingSphere = true;
    public bool showFieldOfView = true;

    [HideInInspector]
    public CinemachineVirtualCamera vC;

    GameObject minimapIcon = null;
    [SerializeField]
    private bool visibleInMiniMap = false;
    Camera mainCamera = null;

    [SerializeField] LayerMask raycastCheckLayer = 0;

    public void Awake()
    {
        vC = GameObject.Find(transform.name + "Camera").GetComponent<CinemachineVirtualCamera>();
        vC.m_Follow = transform.GetChild(transform.childCount - 1);
        playerSoundSubject.AddObserver(this);
        playerSpottedSubject.AddObserver(this);
        GuardGetComponents();
        sensing.SetScriptablesObjects(guardVariables, playerVariables);
        guardMovement.SetScriptablesObjects(guardVariables, playerVariables);
        sensing.GuardSensingAwake();
        guardMovement.GuardMovementAwake();
        guardMovement.SetGuardAndMovementType(guardType, movementType);
        currentColor = guardVariables.patrolColor;

        minimapIcon = transform.GetChild(0).gameObject;
        minimapIcon.SetActive(false);
        visibleInMiniMap = false;
        mainCamera = Camera.main;

        raycastCheckLayer = LayerMask.GetMask("Walls");
    }

    public void Start()
    {
        guardBehavioralTree = new GuardBehaviourTree(this);
        StartCoroutine("Run");
    }

    void OnDrawGizmos()
    {
        if (showSensingSphere)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, guardVariables.fieldOfViewRadius);
        }

        Vector3 fovLine1 = Quaternion.AngleAxis(guardVariables.fieldOfViewAngle, transform.up) * transform.forward * guardVariables.fieldOfViewRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-guardVariables.fieldOfViewAngle, transform.up) * transform.forward * guardVariables.fieldOfViewRadius;

        if (showFieldOfView)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2);
        }
    }

    public void Update()
    {
        if(sensing.playerInSight)
        {
            playerSpottedSubject.NotifyObservers(playerVariables.playerTransform.position);
        }

        ActivateMinimapIconCheck();
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

    public void Notify(SoundType soundType, Vector3 position)
    {
        if (soundType == SoundType.WALKING)
        {
            if (sensing.canHear && !disabled)
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
            if(sensing.CalculateLength(position) <= sensing.sensingCollider.radius)
            {
                sensing.distracted = true;
                guardMovement.SetInvestigationPosition(position);
                guardMovement.ResetIdleTimer();
            }
        }
    }

    public void Notify(Vector3 position)
    {
        if (!disabled)
        {
            if (sensing.CalculateLength(position) <= guardVariables.maxBackupRadius)
            {
                assist = true;
                guardMovement.SetAssistPosition(position);
            }
        }
    }

    void OnDestroy()
    {
        playerSoundSubject.RemoveObserver(this);
        playerSpottedSubject.RemoveObserver(this);
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
        else if(sensing.CheckPlayerInSight())
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

}
