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
        Vector3 lastSightPosition = new Vector3(guard.sensing.playerLastSightPosition.x, 1.0f, guard.sensing.playerLastSightPosition.z);
        float distance = Vector3.Distance(guard.transform.position, lastSightPosition);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.1f)
        {
            guard.UpdateLookingAroundAngle();
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
