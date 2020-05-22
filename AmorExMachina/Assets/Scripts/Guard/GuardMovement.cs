using UnityEngine;
using UnityEngine.AI;
public class GuardMovement : MonoBehaviour
{
    [HideInInspector] public Transform pathHolder = null;
    [HideInInspector] public Vector3[] path = null;
    [HideInInspector] public Vector3 currentWayPoint = Vector3.zero;
    [HideInInspector] public int wayPointIndex = 0;

    [HideInInspector] public float patrolIdleTimer = 5.0f;
    [HideInInspector] public float lookingAroundTimer = 5.0f;

    [HideInInspector] public NavMeshAgent navMeshAgent = null;
    [HideInInspector] public Vector3 investigationPosition = Vector3.zero;
    [HideInInspector] public Vector3 distractionInvestigationPosition = Vector3.zero;
    [HideInInspector] public Vector3 assistPosition = Vector3.zero;

    [HideInInspector] public Quaternion targetRotation = Quaternion.identity;

    private Guard guardScript = null;

    [HideInInspector] public bool idle = false;

    private MovementType movementType = MovementType.WAIT_AFTER_FULL_CYCLE;
    private GuardType guardType = GuardType.MOVING;

    private Transform playerTransform = null;

    //Added 2020-05-20
    private Animator anim;
    [HideInInspector] public bool isWalking = false;
    [HideInInspector] public bool isDisabled = false;
    [HideInInspector] public bool animEnabled = false;

    public void GuardMovementInit()
    {
        SetPath();
        GetComponents();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SetMovementVariables();
    }

    void GetComponents()
    {
        guardScript = GetComponent<Guard>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void SetMovementVariables()
    {
        guardType = guardScript.guardType;
        movementType = guardScript.movementType;
        targetRotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f);
        patrolIdleTimer = guardScript.maxPatrolIdleTimer;
        lookingAroundTimer = guardScript.maxLookingAroundTimer;
    }

    private void Update()
    {
        anim.enabled = animEnabled;
        anim.SetBool("IsWalking", isWalking);
        anim.SetBool("IsDisabled", isDisabled);
    }

    public void FollowPath()
    {
        float distance = Vector3.Distance(transform.position, currentWayPoint);
        if (distance <= navMeshAgent.stoppingDistance + 0.2f)
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

            //Vector3 currentWayPointPosition = currentWayPoint;
            //navMeshAgent.SetDestination(currentWayPointPosition);

            {   //Added 2020-05-22
                Vector3 directionToTransform = newPath.corners[1] - transform.position;

                Quaternion targetQuaternion = Quaternion.LookRotation(directionToTransform);

                transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, guardScript.rotationSpeed * Time.deltaTime);
                navMeshAgent.Move(transform.forward * 0.02f);
            }

            navMeshAgent.speed = guardScript.patrolSpeed;
            navMeshAgent.autoBraking = true;
        }
    }

    public void MoveToLastSightPosition(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        navMeshAgent.speed = guardScript.chaseSpeed;
        navMeshAgent.stoppingDistance = 0.5f;
        navMeshAgent.autoBraking = true;
    }

    public void MoveTowardsKnockedOutGuard(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        navMeshAgent.speed = guardScript.suspiciousSpeed;
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

        NavMeshPath newPath = new NavMeshPath();
        if (navMeshAgent.enabled)
        {
            navMeshAgent.CalculatePath(investigationPosition, newPath);
        }
        if (newPath.status == NavMeshPathStatus.PathComplete)
        {
            navMeshAgent.SetDestination(investigationPosition);
            navMeshAgent.stoppingDistance = 3.0f;
            navMeshAgent.speed = guardScript.suspiciousSpeed;
            navMeshAgent.autoBraking = true;
        }
    }

    public void SetDistractionInvestigationPosition(Vector3 position)
    {
        distractionInvestigationPosition = position;
    }

    public void DistractionInvestigate()
    {
        idle = false;

        NavMeshPath newPath = new NavMeshPath();
        if (navMeshAgent.enabled)
        {
            navMeshAgent.CalculatePath(distractionInvestigationPosition, newPath);
        }
        if (newPath.status == NavMeshPathStatus.PathComplete)
        {
            navMeshAgent.SetDestination(distractionInvestigationPosition);
            navMeshAgent.stoppingDistance = 3.0f;
            navMeshAgent.speed = guardScript.suspiciousSpeed;
            navMeshAgent.autoBraking = true;
        }
    }

    public void ChasePlayer()
    {
        idle = false;

        NavMeshPath newPath = new NavMeshPath();
        if (navMeshAgent.enabled)
        {
            navMeshAgent.CalculatePath(playerTransform.position, newPath);
        }

        if (newPath.status == NavMeshPathStatus.PathComplete)
        {
            navMeshAgent.SetDestination(playerTransform.position);
            navMeshAgent.speed = guardScript.chaseSpeed;
            navMeshAgent.stoppingDistance = 2.5f;
            navMeshAgent.autoBraking = false;
        }
    }

    public void SetAssistPosition(Vector3 position)
    {
        assistPosition = position;
    }

    public void Assist()
    {
        navMeshAgent.SetDestination(assistPosition);
        navMeshAgent.stoppingDistance = 2.5f;
        navMeshAgent.speed = guardScript.chaseSpeed;
        navMeshAgent.autoBraking = true;
    }

    void SetPath()
    {
        path = new Vector3[pathHolder.childCount];
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 wayPointPosition = pathHolder.GetChild(i).position;
            path[i] = wayPointPosition;
        }
        currentWayPoint = path[wayPointIndex];
    }

    public void ResetIdleTimer()
    {
        patrolIdleTimer = guardScript.maxPatrolIdleTimer;
        lookingAroundTimer = guardScript.maxLookingAroundTimer;
    }
}
