using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    public Transform pathHolder;
    [HideInInspector]
    public Vector3[] path;
    [HideInInspector]
    public Vector3 currentWayPoint;
    [HideInInspector]
    public int wayPointIndex = 1;

    [HideInInspector]
    public float talkingTimer = 5.0f;
    [HideInInspector]
    public float idleTimer = 5.0f;

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    [HideInInspector]
    public Vector3 investigationPosition;
    [HideInInspector]
    public Vector3 assistPosition;

    GuardVariables guardVariables;
    PlayerVariables playerVariables;

    public void GuardMovementAwake()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        SetPath();
    }

    public void SetScriptablesObjects(GuardVariables guardVariablesScriptableObject, PlayerVariables playerVariablesScriptableObject)
    {
        guardVariables = guardVariablesScriptableObject;
        playerVariables = playerVariablesScriptableObject;
    }

    public void FollowPath()
    {
        float distance = Vector3.Distance(transform.position, currentWayPoint);
        if (distance <= navMeshAgent.stoppingDistance)
        {
            wayPointIndex = (wayPointIndex + 1) % path.Length;
            if (wayPointIndex == 0)
                navMeshAgent.stoppingDistance = 0.1f;
            else
                navMeshAgent.stoppingDistance = 1.0f;
        }
        currentWayPoint = path[wayPointIndex];
        Vector3 currentWayPointPosition = new Vector3(currentWayPoint.x, 1.0f, currentWayPoint.z);
        navMeshAgent.SetDestination(currentWayPointPosition);
        navMeshAgent.speed = guardVariables.patrolSpeed;
    }

    public void MoveTowardsKnockedOutGuard(Vector3 target)
    {
        Vector3 targetPosition = new Vector3(target.x, 1.0f, target.z);
        navMeshAgent.SetDestination(targetPosition);
        navMeshAgent.speed = guardVariables.suspiciousSpeed;
        navMeshAgent.stoppingDistance = 2.5f;
    }

    public void SetInvestigationPosition(Vector3 position)
    {
        investigationPosition = position;
    }

    public void Investigate()
    {
        Vector3 investigationPos = new Vector3(investigationPosition.x, 1.0f, investigationPosition.z);
        navMeshAgent.SetDestination(investigationPos);
        navMeshAgent.stoppingDistance = 1.0f;
        navMeshAgent.speed = guardVariables.suspiciousSpeed;
    }

    public void ChasePlayer()
    {
        Vector3 playerPos = new Vector3(playerVariables.playerTransform.position.x, 1.0f, playerVariables.playerTransform.position.z);
        navMeshAgent.SetDestination(playerPos);
        navMeshAgent.speed = guardVariables.chaseSpeed;
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

    public void ResetPatrolTimer()
    {
        talkingTimer = guardVariables.maxTalkingTimer;
    }

    public void ResetIdleTimer()
    {
        idleTimer = guardVariables.maxIdletimer;
    }

    public bool CloseGuardNearby()
    {
        float closestDistanceSquared = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Guard guard in GameController.guards)
        {
            if (guard.gameObject != this.gameObject)
            {
                Vector3 directionToGuard = guard.transform.position - currentPosition;
                float distanceSquaredToGuard = directionToGuard.sqrMagnitude;
                if (distanceSquaredToGuard < closestDistanceSquared && distanceSquaredToGuard < 30.0f)
                {
                    closestDistanceSquared = distanceSquaredToGuard;
                    return true;
                }
            }
        }
        return false;
    }
}
