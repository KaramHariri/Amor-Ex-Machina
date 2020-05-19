﻿public class Disabled : Node
{
    Guard guard;

    public Disabled(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (guard.disabled)
        {
            guard.sensing.Reset();
            guard.guardMovement.navMeshAgent.speed = 0.0f;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
