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
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.1f)
        {
            guard.UpdateLookingAroundAngle();
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
