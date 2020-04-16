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
        float distance;
        if (guard.movementType == MovementType.WAIT_AFTER_FULL_CYCLE)
        {
            distance = Vector3.Distance(guard.transform.position, guard.guardMovement.path[0]);
        }
        else
        {
            distance = Vector3.Distance(guard.transform.position, guard.guardMovement.path[guard.guardMovement.wayPointIndex]);
        }
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance && guard.guardMovement.idleTimer > 0 && guard.guardMovement.idle)
        {
            guard.currentColor = Color.Lerp(guard.currentColor, guard.guardVariables.idleColor, Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            guard.guardMovement.idleTimer -= Time.deltaTime;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}