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
            guard.currentColor = Color.Lerp(guard.currentColor, guard.guardVariables.suspiciousColor, Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
