using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardSensing : MonoBehaviour, IGuardDisabledObserver
{
    [HideInInspector] public bool canHear = false;
    [HideInInspector] public bool suspicious = false;
    [HideInInspector] public bool distracted = false;
    private bool playerInRange = false;
    private bool playerInSight = false;

    public bool showHearingRadius = true;
    public bool showFieldOfView = true;

    [Header("Sensing Variables")]
    [SerializeField] private float fieldOfViewRadius = 20.0f;
    [SerializeField] private float hearingRadius = 20.0f;
    [SerializeField] private float fieldOfViewAngle = 70.0f;

    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public SphereCollider sensingCollider;

    /*[HideInInspector]*/ public List<Guard> disabledGuardsFound = null;
    private List<Guard> disabledGuards = null;

    private PlayerVariables playerVariables = null;

    private Guard guardScript = null;

    #region Layer masks
    private LayerMask raycastCheckLayer = 0;
    private LayerMask raycastCheckLayerWithSmoke = 0;
    private LayerMask raycastDisabledGuardCheckLayer = 0;
    #endregion

    [Header("Timers")]
    public float maxDetectionAmount = 2.0f;
    [SerializeField] private float maxTimerSincePlayerWasSpotted = 5.0f;
    [HideInInspector] public float detectionAmount = 0.0f;
    private float timerSincePlayerWasSpotted = 0.0f;

    [Header("Scriptable Objects")]
    public GuardDisabledSubject guardDisabledSubject = null;

    [Header("Distance based values")]
    public float maxDistanceValue = 0.6f;
    public float minDistanceValue = 2.0f;
    private Transform playerTransform;
    private float distancePercent = 0f;
    private float valueDifference = 0f;
    private float distanceFactorAmount = 0f;

    public void GuardSensingAwake()
    {
        GetComponents();
        guardDisabledSubject.AddObserver(this);
        InitLists();
        AssignPlayerVariable();
        AssignLayerMasks();
    }

    void GetComponents()
    {
        sensingCollider = GetComponent<SphereCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        guardScript = GetComponent<Guard>();

        //Added 2020-05-08
        playerTransform = GameObject.Find("Gabriel").GetComponent<Transform>();
    }

    void AssignPlayerVariable()
    {
        playerVariables = guardScript.playerVariables;
    }

    void InitLists()
    {
        disabledGuardsFound = new List<Guard>();
        disabledGuards = new List<Guard>();
    }

    void AssignLayerMasks()
    {
        raycastCheckLayer = LayerMask.GetMask("Walls", "Player");
        raycastCheckLayerWithSmoke = LayerMask.GetMask("Walls", "Player", "SmokeScreen");
        raycastDisabledGuardCheckLayer = LayerMask.GetMask("Walls", "Guards");
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
        SpottedIndicatorHandler();

        if (guardScript.guardState != GuardState.NORMAL) { return; }

        sensingCollider.radius = fieldOfViewRadius;

        UpdateTimerSincePlayerWasSpotted();

        if (playerInRange)
            PlayerInSightCheck();

        DisabledGuardInSightCheck();
    }

    void SpottedIndicatorHandler()
    {
        if (playerInSight)
        {
            {   //These are the things changed for the distance to player sensing //Changed 2020-05-08
                //detectionAmount += Time.deltaTime; 
                CalculateDistanceFactor();
                detectionAmount += Time.deltaTime * distanceFactorAmount;
            }
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
        //if (disabledGuardInRange.Count > 0)
        //{
        //    for (int i = 0; i < disabledGuardInRange.Count; i++)
        //    {
        //        Vector3 directionToDisabledGuard = disabledGuardInRange[i].transform.position - transform.position;
        //        float angle = Vector3.Angle(directionToDisabledGuard, transform.forward);

        //        if (angle < fieldOfViewAngle)
        //        {
        //            if (RaycastHitCheckToTarget(disabledGuardInRange[i].transform, raycastDisabledGuardCheckLayer))
        //            {
        //                if (!disabledGuardsFound.Contains(disabledGuardInRange[i]))
        //                {
        //                    disabledGuardsFound.Add(disabledGuardInRange[i]);
        //                }
        //            }
        //        }
        //    }
        //}

        if(disabledGuards.Count > 0)
        {
            for(int i = 0; i < disabledGuards.Count; i++)
            {
                if (CalculateLength(disabledGuards[i].transform.position) <= sensingCollider.radius)
                {
                    Vector3 directionToDisabledGuard = disabledGuards[i].transform.position - transform.position;
                    float angle = Vector3.Angle(directionToDisabledGuard, transform.forward);

                    if (angle < fieldOfViewAngle)
                    {
                        if (RaycastHitCheckToTarget(disabledGuards[i].transform, raycastDisabledGuardCheckLayer))
                        {
                            if (disabledGuards[i].disabled && !disabledGuards[i].hacked)
                            {
                                if(!disabledGuardsFound.Contains(disabledGuards[i]))
                                    disabledGuardsFound.Add(disabledGuards[i]);
                            }
                            else
                            {
                                if (disabledGuardsFound.Contains(disabledGuards[i]))
                                    disabledGuardsFound.Remove(disabledGuards[i]);
                            }
                        }
                    }
                }
            }
        }
    }

    void SortDisabledGuardsFoundList()
    {
        disabledGuardsFound.Sort(SortByDistance);
    }

    int SortByDistance(Guard guard1Pos, Guard guard2Pos)
    {
        float distance1 = Vector3.Distance(guard1Pos.transform.position, playerVariables.playerTransform.position);
        float distance2 = Vector3.Distance(guard2Pos.transform.position, playerVariables.playerTransform.position);

        if (distance1 < distance2)
        {
            return -1;
        }
        else if (distance1 > distance2)
        {
            return 1;
        }
        return 0;
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
            SortDisabledGuardsFoundList();
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
                    //Debug.DrawLine(transform.position, raycastHit.point);
                    //Debug.Break();
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
        disabledGuardsFound.Clear();
    }

    public void GuardDisabledNotify(Guard disabledGuardScript)
    {
        if (disabledGuardScript != guardScript)
        {
            disabledGuards.Add(disabledGuardScript);
        }
    }

    void OnDestroy()
    {
        guardDisabledSubject.RemoveObserver(this);
    }

    //Added 2020-05-08
    private void CalculateDistanceFactor()
    {
        if(playerTransform == null) { Debug.Log("Can't find player transform"); return; }

        //We probably want to do this in start once we have good values in order to get better preformance, but for now this will allow us to modify the values in real time
        valueDifference = maxDistanceValue - minDistanceValue;

        //The distance for hearing is a bit different from sight so in order to have a proper one for hearing we'd use different metrics. (Hearing doesn't go through walls)
        distancePercent = (Vector3.Distance(playerTransform.position, transform.position) / fieldOfViewRadius);

        distanceFactorAmount = minDistanceValue + distancePercent * valueDifference;
        //Debug.Log("DistanceFactorAmount: " + distanceFactorAmount + " , distancePercent: " + distancePercent);
    }
}
