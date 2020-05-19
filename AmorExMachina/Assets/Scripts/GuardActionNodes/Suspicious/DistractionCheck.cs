using UnityEngine;

public class DistractionCheck : Node
{
    Guard guard;

    public DistractionCheck(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (guard.sensing.Distracted())
        {
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
