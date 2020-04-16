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
            guard.currentColor = Color.Lerp(guard.currentColor, guard.guardVariables.stationaryColor, Time.deltaTime);
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}