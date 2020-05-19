using UnityEngine;

public class StationaryGuardCheck : Node
{
    public Guard guard;

    public StationaryGuardCheck(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if(guard.guardType == GuardType.STATIONARY)
        {
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}