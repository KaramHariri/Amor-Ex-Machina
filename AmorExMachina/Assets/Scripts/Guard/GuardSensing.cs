using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardSensing : MonoBehaviour, IGuardDisabledObserver
{
    /*[HideInInspector]*/ public bool canHear = false;
    [HideInInspector] public bool suspicious = false;
    [HideInInspector] public bool distracted = false;
    [SerializeField] private bool playerInRange = false;
    [SerializeField] private bool playerInSight = false;
    public bool showSensingSphere = true;
    public bool showFieldOfView = true;

    [SerializeField] private float fieldOfViewRadius = 20.0f;
    [SerializeField] private float fieldOfViewAngle = 70.0f;

    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public SphereCollider sensingCollider;

    [HideInInspector] public List<Guard> disabledGuardsFound = null;
    private List<Guard> disabledGuardInRange = null;

    private GuardVariables guardVariables = null;
    private PlayerVariables playerVariables = null;
    public GuardDisabledSubject guardDisabledSubject = null;

    private Guard guardScript = null;

    #region Layer masks
    private LayerMask raycastCheckLayer = 0;
    private LayerMask raycastCheckLayerWithSmoke = 0;
    private LayerMask raycastDisabledGuardCheckLayer = 0;
    #endregion

    #region Timers
    [HideInInspector] public float detectionAmount = 0.0f;
    public float maxDetectionAmount = 2.0f;
    [SerializeField] private float maxTimerSincePlayerWasSpotted = 5.0f;
    [SerializeField] private float timerSincePlayerWasSpotted = 0.0f;
    #endregion

    public void GuardSensingAwake()
    {
        GetComponents();
        guardDisabledSubject.AddObserver(this);
        InitLists();
        AssignGuardAndPlayerVariables();
        AssignLayerMasks();
    }

    void GetComponents()
    {
        sensingCollider = GetComponent<SphereCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        guardScript = GetComponent<Guard>();
    }

    void AssignGuardAndPlayerVariables()
    {
        guardVariables = guardScript.guardVariables;
        playerVariables = guardScript.playerVariables;
    }

    void InitLists()
    {
        disabledGuardsFound = new List<Guard>();
        disabledGuardInRange = new List<Guard>();
    }

    void AssignLayerMasks()
    {
        raycastCheckLayer = LayerMask.GetMask("Walls", "Player");
        raycastCheckLayerWithSmoke = LayerMask.GetMask("Walls", "Player", "SmokeScreen");
        raycastDisabledGuardCheckLayer = LayerMask.GetMask("Walls", "Guards");
    }

    void OnDrawGizmos()
    {
        if (showSensingSphere)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, fieldOfViewRadius);
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
        guardDisabledSubject.GuardDisabledNotify(guardScript, guardScript.disabled, guardScript.hacked);

        if (guardScript.guardState != GuardState.NORMAL) { return; }

        sensingCollider.radius = fieldOfViewRadius;
        SpottedIndicatorHandler();

        UpdateTimerSincePlayerWasSpotted();

        if (playerInRange)
            PlayerInSightCheck();

        DisabledGuardInSightCheck();
    }

    void SpottedIndicatorHandler()
    {
        if (playerInSight)
        {
            detectionAmount += Time.deltaTime;
            if (detectionAmount >= maxDetectionAmount)
                detectionAmount = maxDetectionAmount;

            UIManager.createIndicator(this.transform);
            UIManager.updateIndicator(this.transform, IndicatorColor.Red);
        }
        else if (suspicious)
        {
            detectionAmount += Time.deltaTime * 2.0f;
            if (detectionAmount >= maxDetectionAmount)
                detectionAmount = maxDetectionAmount;

            UIManager.createIndicator(this.transform);
            UIManager.updateIndicator(this.transform, IndicatorColor.Yellow);
        }
        else
        {
            detectionAmount -= Time.deltaTime;
            if (detectionAmount <= 0.0f)
            {
                detectionAmount = 0.0f;
                UIManager.removeIndicator(this.transform);
            }
            else
                UIManager.updateIndicator(this.transform, IndicatorColor.Yellow);
        }
    }

    void UpdateTimerSincePlayerWasSpotted()
    {
        timerSincePlayerWasSpotted += Time.deltaTime;
        timerSincePlayerWasSpotted = Mathf.Clamp(timerSincePlayerWasSpotted, 0.0f, 20.0f);
    }

    void PlayerInSightCheck()
    {
        if (!guardScript.hacked && !guardScript.disabled)
        {
            if(timerSincePlayerWasSpotted < maxTimerSincePlayerWasSpotted)
            {
                if (RaycastHitCheckToTarget(playerVariables.playerTransform, raycastCheckLayer))
                {
                    timerSincePlayerWasSpotted = 0.0f;
                    playerInSight = true;
                }
                else
                {
                    playerInSight = false;
                }
            }
            else
            {
                if (RaycastHitCheckToTarget(playerVariables.playerTransform, raycastCheckLayerWithSmoke))
                {
                    timerSincePlayerWasSpotted = 0.0f;
                    playerInSight = true;
                }
                else
                {
                    playerInSight = false;
                }
            }
        }
    }

    void DisabledGuardInSightCheck()
    {
        if (disabledGuardInRange.Count > 0)
        {
            for (int i = 0; i < disabledGuardInRange.Count; i++)
            {
                Vector3 directionToDisabledGuard = disabledGuardInRange[i].transform.position - transform.position;
                float angle = Vector3.Angle(directionToDisabledGuard, transform.forward);

                if (angle < fieldOfViewAngle)
                {
                    if (RaycastHitCheckToTarget(disabledGuardInRange[i].transform, raycastDisabledGuardCheckLayer))
                    {
                        if (!disabledGuardsFound.Contains(disabledGuardInRange[i]))
                        {
                            disabledGuardsFound.Add(disabledGuardInRange[i]);
                        }
                    }
                }
            }
        }
    }

    public bool PlayerDetectedCheck()
    {
        if (playerInSight && detectionAmount >= maxDetectionAmount)
        {
            return true;
        }
        return false;
    }

    public bool Suspicious()
    {
        if ((suspicious && detectionAmount >= maxDetectionAmount) || distracted)
        {
            return true;
        }
        return false;
    }

    public bool FoundKnockedOutGuard()
    {
        if (disabledGuardsFound.Count > 0)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            canHear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSight = false;
            playerInRange = false;
            canHear = false;
        }
    }

    bool RaycastHitCheckToTarget(Transform target, LayerMask layerMask)
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + 0.5f, target.position.z);
        Vector3 directionToTarget = targetPosition - transform.position;
        float angle = Vector3.Angle(directionToTarget, transform.forward);

        if (angle < fieldOfViewAngle)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, directionToTarget.normalized, out raycastHit, sensingCollider.radius, layerMask))
            {
                if (raycastHit.collider.transform == target.transform)
                {
                    return true;   
                }
            }
        }
        return false;
    }

    public float CalculateLength(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        if (navMeshAgent.enabled)
            navMeshAgent.CalculatePath(targetPosition, path);

        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

        allWayPoints[0] = transform.position;
        allWayPoints[allWayPoints.Length - 1] = targetPosition;


        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }

        float pathLength = 0.0f;
        for (int i = 0; i < allWayPoints.Length - 1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }

        return pathLength;
    }

    public void Reset()
    {
        playerInSight = false;
        suspicious = false;
    }

    public void GuardDisabledNotify(Guard disabledGuardScript, bool isDisabled, bool isHacked)
    {
        if (disabledGuardScript != guardScript)
        {
            Vector3 directionToDisabledGuard = disabledGuardScript.transform.position - transform.position;
            if(directionToDisabledGuard.magnitude <= sensingCollider.radius && !isHacked && isDisabled)
            {
                if(!disabledGuardInRange.Contains(disabledGuardScript))
                {
                    disabledGuardInRange.Add(disabledGuardScript);
                }
            }
            else
            {
                if(disabledGuardInRange.Contains(disabledGuardScript) || isHacked || !isDisabled)
                {
                    disabledGuardInRange.Remove(disabledGuardScript);
                }
            }
        }
    }

    void OnDestroy()
    {
        guardDisabledSubject.RemoveObserver(this);
    }
}
