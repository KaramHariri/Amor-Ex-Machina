public class Disabled : Node
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
            guard.currentColor = UnityEngine.Color.Lerp(guard.currentColor, guard.guardVariables.disabledColor, UnityEngine.Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            guard.sensing.Reset();
            guard.guardMovement.navMeshAgent.speed = 0.0f;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
