using UnityEngine;

public class LastSightPositionLookAround : Node
{
    Guard guard;

    public LastSightPositionLookAround(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.idleTimer -= Time.deltaTime;
        if (guard.guardMovement.idleTimer <= 0)
        {
            guard.sensing.playerWasInSight = false;
            guard.guardMovement.idleTimer = 5.0f;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
