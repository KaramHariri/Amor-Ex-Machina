using UnityEngine;

public class AlarmedCheck : Node
{
    Guard guard;

    public AlarmedCheck(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (guard.sensing.Alarmed())
        {
            guard.guardMovement.animEnabled = true;
            guard.guardMovement.isWalking = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
