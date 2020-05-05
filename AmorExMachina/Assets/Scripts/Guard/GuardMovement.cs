using UnityEngine;
using UnityEngine.AI;
public class GuardMovement : MonoBehaviour
{
    [HideInInspector] public Transform pathHolder = null;
    [HideInInspector] public Vector3[] path = null;
    [HideInInspector] public Vector3 currentWayPoint = Vector3.zero;
    [HideInInspector] public int wayPointIndex = 0;

    //[HideInInspector]
    public float idleTimer = 5.0f;

    [HideInInspector] public NavMeshAgent navMeshAgent = null;
    [HideInInspector] public Vector3 investigationPosition = Vector3.zero;
    [HideInInspector] public Vector3 assistPosition = Vector3.zero;

    [HideInInspector] public Quaternion targetRotation = Quaternion.identity;

    private GuardVariables guardVariables = null;
    private PlayerVariables playerVariables = null;
    private Guard guardScript = null;

    [HideInInspector] public bool idle = false;

    private MovementType movementType = MovementType.WAIT_AFTER_FULL_CYCLE;
    private GuardType guardType = GuardType.MOVING;
    private GuardState guardState = GuardState.NORMAL;

    public void GuardMovementAwake()
    {
        SetPath();
        GetComponents();
        AssignPlayerAndGuardVariables();
        SetMovementVariables();

        guardState = guardScript.guardState;
    }

    void GetComponents()
    {
        guardScript = GetComponent<Guard>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void AssignPlayerAndGuardVariables()
    {
        guardVariables = guardScript.guardVariables;
        playerVariables = guardScript.playerVariables;
    }

    void SetMovementVariables()
    {
        guardType = guardScript.guardType;
        movementType = guardScript.movementType;
        targetRotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f);
        idleTimer = guardVariables.maxIdletimer;
    }

    public void FollowPath()
    {
        float distance = Vector3.Distance(transform.position, currentWayPoint);
        if (distance <= navMeshAgent.stoppingDistance)
        {
            if (guardType == GuardType.STATIONARY)
            {
                wayPointIndex = 0;
                idle = true;
                navMeshAgent.stoppingDistance = 0.1f;
            }
            else
            {
                switch (movementType)
                {
                    case MovementType.WAIT_AFTER_FULL_CYCLE:
                        wayPointIndex = (wayPointIndex + 1) % path.Length;
                        if (wayPointIndex == 0)
                        {
                            idle = true;
                            navMeshAgent.stoppingDistance = 0.1f;
                        }
                        else
                            navMeshAgent.stoppingDistance = 1.0f;
                        break;
                    case MovementType.WAIT_AT_WAYPOINT:
                        wayPointIndex = (wayPointIndex + 1) % path.Length;
                        navMeshAgent.stoppingDistance = 0.1f;
                        idle = true;
                        break;
                    case MovementType.DONT_WAIT:
                        wayPointIndex = (wayPointIndex + 1) % path.Length;
                        navMeshAgent.stoppingDistance = 0.5f;
                        break;
                }
            }
            
        }
        NavMeshPath newPath = new NavMeshPath();
        if (navMeshAgent.enabled)
        {
            navMeshAgent.CalculatePath(path[wayPointIndex], newPath);
        }

        if (newPath.status == NavMeshPathStatus.PathComplete)
        {
            currentWayPoint = path[wayPointIndex];
            Vector3 currentWayPointPosition = new Vector3(currentWayPoint.x, 1.0f, currentWayPoint.z);
            navMeshAgent.SetDestination(currentWayPointPosition);
            navMeshAgent.speed = guardVariables.patrolSpeed;
            navMeshAgent.autoBraking = true;
        }
    }

    public void MoveTowardsKnockedOutGuard(Vector3 target)
    {
        Vector3 targetPosition = new Vector3(target.x, 1.0f, target.z);
        navMeshAgent.SetDestination(targetPosition);
        navMeshAgent.speed = guardVariables.suspiciousSpeed;
        navMeshAgent.stoppingDistance = 2.5f;
        navMeshAgent.autoBraking = true;
    }

    public void SetInvestigationPosition(Vector3 position)
    {
        investigationPosition = position;
    }

    public void Investigate()
    {
        idle = false;
        Vector3 investigationPos = new Vector3(investigationPosition.x, 1.0f, investigationPosition.z);
        navMeshAgent.SetDestination(investigationPos);
        navMeshAgent.stoppingDistance = 3.0f;
        navMeshAgent.speed = guardVariables.suspiciousSpeed;
        navMeshAgent.autoBraking = true;
    }

    public void ChasePlayer()
    {
        idle = false;
        Vector3 playerPos = new Vector3(playerVariables.playerTransform.position.x, 1.0f, playerVariables.playerTransform.position.z);
        navMeshAgent.SetDestination(playerPos);
        navMeshAgent.speed = guardVariables.chaseSpeed;
        navMeshAgent.stoppingDistance = 2.5f;
        navMeshAgent.autoBraking = false;
    }

    public void SetAssistPosition(Vector3 position)
    {
        assistPosition = position;
    }

    public void Assist()
    {
        Vector3 assistPos = new Vector3(assistPosition.x, 1.0f, assistPosition.z);
        navMeshAgent.SetDestination(assistPos);
        navMeshAgent.stoppingDistance = 2.5f;
        navMeshAgent.speed = guardVariables.chaseSpeed;
        navMeshAgent.autoBraking = true;
    }

    void SetPath()
    {
        path = new Vector3[pathHolder.childCount];
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 wayPointPosition = new Vector3(pathHolder.GetChild(i).position.x, 1.0f, pathHolder.GetChild(i).position.z);
            path[i] = wayPointPosition;
        }
        currentWayPoint = path[wayPointIndex];
    }

    public void ResetIdleTimer()
    {
        idleTimer = guardVariables.maxIdletimer;
    }
}
