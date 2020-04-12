using System.Collections;
using UnityEngine;

public class Guard : MonoBehaviour, IPlayerSoundObserver, IPlayerLastSightPositionObserver
{
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
    public bool beingControlled = false;
    [HideInInspector]
    public bool assist = false;

    public PlayerSoundSubject playerSoundSubject;
    public PlayerLastSightPositionSubject playerLastSightPositionSubject;
    public GuardVariables guardVariables;
    public PlayerVariables playerVariables;

    public void Awake()
    {
        playerSoundSubject.AddObserver(this);
        playerLastSightPositionSubject.AddObserver(this);
        GuardGetComponents();
        sensing.SetScriptablesObjects(guardVariables, playerVariables);
        guardMovement.SetScriptablesObjects(guardVariables, playerVariables);
        sensing.GuardSensingAwake();
        guardMovement.GuardMovementAwake();
        currentColor = guardVariables.patrolColor;
    }

    public void Start()
    {
        guardBehavioralTree = new GuardBehaviourTree(this);
        StartCoroutine("Run");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, guardVariables.fieldOfViewRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(guardVariables.fieldOfViewAngle, transform.up) * transform.forward * guardVariables.fieldOfViewRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-guardVariables.fieldOfViewAngle, transform.up) * transform.forward * guardVariables.fieldOfViewRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
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
