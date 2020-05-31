using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardSensing : MonoBehaviour, IGuardDisabledObserver
{
    // Sensing Variables.
    [HideInInspector] public bool canHear = false;
    [HideInInspector] public bool suspicious = false;
    [HideInInspector] public bool distracted = false;
    [HideInInspector] public bool alarmed = false;
    [HideInInspector] public bool playerWasInSight = false;
    private bool playerInRange = false;
    private bool playerInSight = false;

    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public SphereCollider sensingCollider;

    // Disabled Guard lists.
    [HideInInspector] public List<Guard> disabledGuardsFound = null;
    private List<Guard> disabledGuards = null;

    private Guard guardScript = null;

    [HideInInspector] public Vector3 playerLastSightPosition = Vector3.zero;

    #region Layer masks
    private LayerMask raycastCheckLayer = 0;
    private LayerMask raycastCheckLayerWithSmoke = 0;
    private LayerMask raycastDisabledGuardCheckLayer = 0;
    #endregion
    
    // Timers.
    [HideInInspector] public float detectionAmount = 0.0f;
    [HideInInspector] public float maxDetectionAmount = 0.0f;
    private float timerSincePlayerWasSpotted = 0.0f;

    private GuardDisabledSubject guardDisabledSubject = null;

    // Variables for modifying the indicator fill speed.
    private float distancePercent = 0f;
    private float valueDifference = 0f;
    private float distanceFactorAmount = 0f;

    private AudioManager audioManager = null;

    private Transform playerTransform;

    void Awake()
    {
        GetComponents();
        InitLists();
        AssignLayerMasks();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        GetStaticReferencesFromGameHandler();
        StartCoroutine("GuardHearingRangeHandler");
    }

    void GetComponents()
    {
        sensingCollider = GetComponent<SphereCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        guardScript = GetComponent<Guard>();
    }

    void InitLists()
    {
        disabledGuardsFound = new List<Guard>();
        disabledGuards = new List<Guard>();
    }

    void GetStaticReferencesFromGameHandler()
    {
        guardDisabledSubject = GameHandler.guardDisabledSubject;
        if (guardDisabledSubject == null)
        {
            Debug.Log("GuardSensing can't find GuardDisabledSubject in GameHandler");
        }
        guardDisabledSubject.AddObserver(this);

        audioManager = GameHandler.audioManager;
        if (audioManager == null)
        {
            Debug.Log("GuardSensing can't find AudioManager in GameHandler");
        }
    }

    void AssignLayerMasks()
    {
        raycastCheckLayer = LayerMask.GetMask("Walls", "Player");
        raycastCheckLayerWithSmoke = LayerMask.GetMask("Walls", "Player", "SmokeScreen");
        raycastDisabledGuardCheckLayer = LayerMask.GetMask("Walls", "Guards");
    }

    public void Update()
    {
        SpottedIndicatorHandler();

        if (guardScript.guardState != GuardState.NORMAL) { return; }

        sensingCollider.radius = guardScript.fieldOfViewRadius;

        UpdateTimerSincePlayerWasSpotted();

        if (playerInRange)
            PlayerInSightCheck();

        DisabledGuardInSightCheck();
    }

    // Update the fill of the bar depending on the guard state.
    void SpottedIndicatorHandler()
    {
        // Check if the player is or was in the sight of the guard and the guard is not disabled.
        if ((playerInSight || playerWasInSight) && !guardScript.disabled)
        {
            {   //These are the things changed for the distance to player sensing //Changed 2020-05-08
                //detectionAmount += Time.deltaTime; 
                CalculateDistanceFactor();
                detectionAmount += Time.deltaTime * distanceFactorAmount;
            }
            if (detectionAmount >= guardScript.maxDetectionBarAmount)
                detectionAmount = guardScript.maxDetectionBarAmount;

            // Activate an indicator from the pool of indicators if there is no indicator activated for this guard.
            UIManager.createIndicator(this.transform);
            // Update the indicator fill value and color. ( Increase the fill)
            UIManager.updateIndicator(this.transform, IndicatorColor.Red);
        }
        // Check if the guard heard something (Player made noise) and the guard is not disabled.
        else if ((suspicious || alarmed) && !guardScript.disabled)
        {
            detectionAmount += Time.deltaTime * 2.0f;
            if (detectionAmount >= guardScript.maxDetectionBarAmount)
                detectionAmount = guardScript.maxDetectionBarAmount;

            // Activate an indicator from the pool of indicators if there is no indicator activated for this guard.
            UIManager.createIndicator(this.transform);
            // Update the indicator fill value and color. (Increase the fill)
            UIManager.updateIndicator(this.transform, IndicatorColor.Yellow);
        }
        else
        {
            detectionAmount -= Time.deltaTime;
            if (detectionAmount <= 0.0f)
            {
                detectionAmount = 0.0f;
                // Remove an indicator if there is an indicator is activated for this guard.
                UIManager.removeIndicator(this.transform);
            }
            else
            {
                // Update the fill of the indicator. (Decrease the fill)
                UIManager.updateIndicator(this.transform, IndicatorColor.Yellow);
            }
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
            if(timerSincePlayerWasSpotted < guardScript.maxTimerSincePlayerWasSpotted)
            {
                if (RaycastHitCheckToTarget(playerTransform, raycastCheckLayer))
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
                if (RaycastHitCheckToTarget(playerTransform, raycastCheckLayerWithSmoke))
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

    // Increase/Decrease the guard hearing range depending if the guard is visible to the player.
    IEnumerator GuardHearingRangeHandler()
    {
        while (true)
        {
            if (!Suspicious() && !playerWasDetectedCheck() && !PlayerDetectedCheck() && !guardScript.disabled && !guardScript.hacked)
            {
                if (HearingSenseRaycastCheck())
                {
                    guardScript.hearingRadius = guardScript.maxHearingRadius;
                }
                else
                {
                    guardScript.hearingRadius = guardScript.minHearingRadius;
                }
            }
            else if(Suspicious() || playerWasDetectedCheck() || PlayerDetectedCheck())
            {
                guardScript.hearingRadius = guardScript.alertedHearingRadius;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    void DisabledGuardInSightCheck()
    {
        if(disabledGuards.Count > 0)
        {
            for(int i = 0; i < disabledGuards.Count; i++)
            {
                if (CalculateLength(disabledGuards[i].transform.position) <= sensingCollider.radius)
                {
                    Vector3 directionToDisabledGuard = disabledGuards[i].transform.position - transform.position;
                    float angle = Vector3.Angle(directionToDisabledGuard, guardScript.guardNeckTransform.forward);

                    if (angle < guardScript.fieldOfViewAngle)
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

    // Sort the disabled guard that are found by distance to this guard position.
    int SortByDistance(Guard guard1Pos, Guard guard2Pos)
    {
        float distance1 = Vector3.Distance(guard1Pos.transform.position, transform.position);
        float distance2 = Vector3.Distance(guard2Pos.transform.position, transform.position);

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
        if (playerInSight && detectionAmount >= guardScript.maxDetectionBarAmount)
        {
            playerWasInSight = true;
            suspicious = false;
            playerLastSightPosition = playerTransform.position;
            return true;
        }
        return false;
    }

    public bool playerWasDetectedCheck()
    {
        if (playerWasInSight && !suspicious)
        {
            return true;
        }
        return false;
    }

    public bool Suspicious()
    {
        if (suspicious && detectionAmount >= guardScript.maxDetectionBarAmount)
        {
            return true;
        }
        return false;
    }

    public bool Alarmed()
    {
        if(alarmed)
        {
            return true;
        }
        return false;
    }

    public bool Distracted()
    {
        if(distracted)
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
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0.0f;
        Vector3 raycastOrigin = new Vector3(transform.position.x, 1.0f, transform.position.z);
        float angle = Vector3.Angle(directionToTarget, guardScript.guardNeckTransform.forward);
        if (angle < guardScript.fieldOfViewAngle)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(raycastOrigin, directionToTarget.normalized, out raycastHit, sensingCollider.radius, layerMask))
            {
                if (raycastHit.collider.transform == target.transform)
                {
                    Debug.DrawRay(raycastOrigin, directionToTarget);
                    return true;
                }
            }
        }
        return false;
    }

    //bool RaycastHitCheckToTarget(Transform target, LayerMask layerMask)
    //{
    //    Vector3 targetPosition = new Vector3(target.position.x, target.position.y + 0.5f, target.position.z);
    //    Vector3 directionToTarget = targetPosition - transform.position;
    //    float angle = Vector3.Angle(directionToTarget, guardScript.guardNeckTransform.forward);

    //    if (angle < guardScript.fieldOfViewAngle)
    //    {
    //        RaycastHit raycastHit;
    //        if (Physics.Raycast(transform.position, directionToTarget.normalized, out raycastHit, sensingCollider.radius, layerMask))
    //        {
    //            if (raycastHit.collider.transform == target.transform)
    //            {
    //                return true;   
    //            }
    //        }
    //    }
    //    return false;
    //}

    bool HearingSenseRaycastCheck()
    {
        Vector3 playerPosition = new Vector3(playerTransform.position.x, playerTransform.position.y + 0.5f, playerTransform.position.z);
        Vector3 directionToTarget = playerPosition - transform.position;
        float angle = Vector3.Angle(directionToTarget, guardScript.guardNeckTransform.forward);

        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, directionToTarget.normalized, out raycastHit, sensingCollider.radius, raycastCheckLayer))
        {
            if (raycastHit.collider.transform == playerTransform)
            {
                return true;
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
        playerWasInSight = false;
        guardScript.guardMovement.idle = false;
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
        valueDifference = guardScript.maxDistanceValue - guardScript.minDistanceValue;

        //The distance for hearing is a bit different from sight so in order to have a proper one for hearing we'd use different metrics. (Hearing doesn't go through walls)
        distancePercent = (Vector3.Distance(playerTransform.position, transform.position) / guardScript.fieldOfViewRadius);

        distanceFactorAmount = guardScript.minDistanceValue + distancePercent * valueDifference;
        //Debug.Log("DistanceFactorAmount: " + distanceFactorAmount + " , distancePercent: " + distancePercent);
    }

    public void NotifyBeingDisabledFromLoad()
    {
        guardDisabledSubject.GuardDisabledNotify(guardScript);
    }
}
