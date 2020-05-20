using UnityEngine;

public class PlayerWasInSightCheck : Node
{
    Guard guard;

    public PlayerWasInSightCheck(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (guard.sensing.playerWasDetectedCheck())
        {
            guard.guardMovement.isWalking = true;
            guard.assist = false;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
