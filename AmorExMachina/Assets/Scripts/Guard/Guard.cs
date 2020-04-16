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

public class Guard : MonoBehaviour, IPlayerSoundObserver, IPlayerLastSightPositionObserver
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
    public PlayerLastSightPositionSubject playerLastSightPositionSubject;
    public GuardVariables guardVariables;
    public PlayerVariables playerVariables;

    public bool showSensingSphere = true;
    public bool showFieldOfView = true;

    [HideInInspector]
    public CinemachineVirtualCamera vC;

    public void Awake()
    {
        vC = GameObject.Find(transform.name + "Camera").GetComponent<CinemachineVirtualCamera>();
        vC.m_Follow = transform.GetChild(transform.childCount - 1);
        playerSoundSubject.AddObserver(this);
        playerLastSightPositionSubject.AddObserver(this);
        GuardGetComponents();
        sensing.SetScriptablesObjects(guardVariables, playerVariables);
        guardMovement.SetScriptablesObjects(guardVariables, playerVariables);
        sensing.GuardSensingAwake();
        guardMovement.GuardMovementAwake();
        guardMovement.SetGuardAndMovementType(guardType, movementType);
        currentColor = guardVariables.patrolColor;
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
            playerLastSightPositionSubject.NotifyObservers(playerVariables.playerTransform.position);
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
            Debug.Log("Distraction");
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
        playerLastSightPositionSubject.RemoveObserver(this);
    }
}
