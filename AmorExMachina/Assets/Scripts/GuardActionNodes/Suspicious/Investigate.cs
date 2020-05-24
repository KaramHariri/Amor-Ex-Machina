using UnityEngine;

public class Investigate : Node
{
    Guard guard;

    public Investigate(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.Investigate();
        float distance = Vector3.Distance(guard.transform.position, guard.guardMovement.investigationPosition);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.2f)
        {
            guard.guardMovement.idle = true;
            //guard.UpdateLookingAroundAngle();
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}