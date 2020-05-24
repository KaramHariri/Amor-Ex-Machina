using UnityEngine;

public class MoveToLastSightPosition : Node
{
    Guard guard;

    public MoveToLastSightPosition(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.MoveToLastSightPosition(guard.sensing.playerLastSightPosition);
        float distance = Vector3.Distance(guard.transform.position, guard.sensing.playerLastSightPosition);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.3f)
        {
            guard.guardMovement.idle = true;
            guard.guardMovement.reachedDestination = true;
            //guard.UpdateLookingAroundAngle();
            nodeState = NodeState.SUCCESS;
        }
        else
        {
            guard.guardMovement.reachedDestination = false;
        }
        return nodeState;
    }
}
