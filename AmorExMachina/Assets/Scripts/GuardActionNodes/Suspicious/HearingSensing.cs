public class HearingSensing : Node
{
    Guard guard;

    public HearingSensing(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (guard.sensing.Suspicious())
        {
            guard.currentColor = UnityEngine.Color.Lerp(guard.currentColor, guard.guardVariables.suspiciousColor, UnityEngine.Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
