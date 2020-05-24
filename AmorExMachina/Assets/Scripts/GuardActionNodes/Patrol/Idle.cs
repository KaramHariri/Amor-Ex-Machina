using UnityEngine;

public class Idle : Node
{
    Guard guard;

    public Idle(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        float distance = float.PositiveInfinity;
        if (guard.movementType == MovementType.WAIT_AFTER_FULL_CYCLE && guard.guardMovement.wayPointIndex == 0)
        {
            Vector3 firstWayPointPosition = new Vector3(guard.guardMovement.path[0].x, -0.2f, guard.guardMovement.path[0].z);
            distance = Vector3.Distance(guard.transform.position, firstWayPointPosition);
            //distance = Vector3.Distance(guard.transform.position, guard.guardMovement.path[0]);
        }
        else
        {
            distance = Vector3.Distance(guard.transform.position, guard.guardMovement.path[guard.guardMovement.wayPointIndex]);
        }

        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.3f && guard.guardMovement.patrolIdleTimer > 0f && guard.guardMovement.shouldBeIdle /*&& guard.guardMovement.idle*/)
        {
            guard.guardMovement.idle = true;
            //guard.guardMovement.animEnabled = false;
            guard.guardMovement.isWalking = false;
            guard.guardMovement.patrolIdleTimer -= Time.deltaTime;
            if(guard.guardMovement.patrolIdleTimer <= 0.0f)
            {
                guard.guardMovement.idle = false;
                guard.guardMovement.shouldBeIdle = false;
            }
            nodeState = NodeState.SUCCESS;
        }
        else
        {
            //guard.guardMovement.animEnabled = true;
            guard.guardMovement.isWalking = true;
        }
        return nodeState;
    }
}