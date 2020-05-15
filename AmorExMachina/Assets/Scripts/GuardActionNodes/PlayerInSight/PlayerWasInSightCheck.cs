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
            guard.assist = false;
            guard.currentColor = Color.Lerp(guard.currentColor, guard.guardVariables.chasingColor, Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
