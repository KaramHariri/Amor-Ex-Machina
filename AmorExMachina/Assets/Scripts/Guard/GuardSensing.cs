using System.Collections.Generic;
using UnityEngine;

public class GuardSensing : MonoBehaviour
{
    [HideInInspector]
    public bool playerInSight = false;
    [HideInInspector]
    public bool canHear = false;
    [HideInInspector]
    public bool suspicious = false;
    [HideInInspector]
    public bool distracted = false;

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    [HideInInspector]
    public SphereCollider sensingCollider;

    [HideInInspector]
    public List<Guard> disabledGuards;

    GuardVariables guardVariables;
    PlayerVariables playerVariables;

    Guard guardScript;

    [SerializeField] LayerMask raycastCheckLayer = 0;
    [SerializeField] LayerMask raycastCheckLayerWithSmoke = 0;

    [HideInInspector]
    public float detectionAmount = 0.0f;
    public float maxDetectionAmount = 2.0f;
    [SerializeField]
    private float timerSincePlayerWasSpotted = 0.0f;
    [SerializeField]
    private float maxTimerSincePlayerWasSpotted = 5.0f;

    public void GuardSensingAwake()
    {
        sensingCollider = GetComponent<SphereCollider>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        guardScript = GetComponent<Guard>();
        raycastCheckLayer = LayerMask.GetMask("Walls", "Player");
        raycastCheckLayerWithSmoke = LayerMask.GetMask("Walls", "Player", "SmokeScreen");
    }

    public void Update()
    {
        sensingCollider.radius = guardVariables.fieldOfViewRadius;
        SpottedIndicatorHandler();

        timerSincePlayerWasSpotted += Time.deltaTime;
        timerSincePlayerWasSpotted = Mathf.Clamp(timerSincePlayerWasSpotted, 0.0f, 20.0f);
    }

    public void SetScriptablesObjects(GuardVariables guardVariablesScriptableObject, PlayerVariables playerVariablesScriptableObject)
    {
        guardVariables = guardVariablesScriptableObject;
        playerVariables = playerVariablesScriptableObject;
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

    public bool CheckPlayerInSight()
    {
        if (playerInSight && detectionAmount >= maxDetectionAmount)
            return true;

        return false;
    }

    public bool Suspicious()
    {
        if (suspicious || distracted)
        {
            return true;
        }
        return false;
    }

    public bool FoundKnockedOutGuard()
    {
        if (disabledGuards.Count > 0)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerVariables.playerTransform.gameObject)
        {
            if (!guardScript.beingControlled && !guardScript.disabled)
            {
                canHear = true;
                playerInSight = false;

                Vector3 playerPosition = new Vector3(other.transform.position.x, other.transform.position.y + 0.5f, other.transform.position.z);
                Vector3 direction = playerPosition - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);

                if (angle < guardVariables.fieldOfViewAngle)
                {
                    RaycastHit raycastHit;
                    if (timerSincePlayerWasSpotted < maxTimerSincePlayerWasSpotted)
                    {
                        if (Physics.Raycast(transform.position, direction.normalized, out raycastHit, sensingCollider.radius, raycastCheckLayer))
                        {
                            if (raycastHit.collider.gameObject == playerVariables.playerTransform.gameObject)
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
                    else
                    {
                        if (Physics.Raycast(transform.position, direction.normalized, out raycastHit, sensingCollider.radius, raycastCheckLayerWithSmoke))
                        {
                            if (raycastHit.collider.gameObject == playerVariables.playerTransform.gameObject)
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
            }
        }

        if (other.gameObject.CompareTag("Guard") && other.gameObject != this.gameObject)
        {
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < guardVariables.fieldOfViewAngle)
            {
                if (other.GetComponent<Guard>().disabled && !other.GetComponent<Guard>().beingControlled)
                {
                    RaycastHit raycastHit;
                    //if (Physics.Raycast(transform.position, direction.normalized, out raycastHit, sensingCollider.radius))
                    if (Physics.Raycast(transform.position, direction.normalized, out raycastHit, sensingCollider.radius, raycastCheckLayer))
                    {
                        if (raycastHit.collider.gameObject == other.gameObject)
                        {
                            if (!disabledGuards.Contains(other.GetComponent<Guard>()))
                                disabledGuards.Add(other.GetComponent<Guard>());
                        }
                    }
                }
                else
                {
                    if (disabledGuards.Contains(other.GetComponent<Guard>()))
                        disabledGuards.Remove(other.GetComponent<Guard>());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerVariables.playerTransform.gameObject)
        {
            playerInSight = false;
            canHear = false;
        }
    }

    public float CalculateLength(Vector3 targetPosition)
    {
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
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
        canHear = false;
        suspicious = false;
    }
}
