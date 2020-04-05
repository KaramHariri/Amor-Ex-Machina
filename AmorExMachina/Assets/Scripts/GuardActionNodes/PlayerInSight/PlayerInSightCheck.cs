public class PlayerInSightCheck : Node
{
    Guard guard;

    public PlayerInSightCheck(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (guard.sensing.CheckPlayerInSight())
        {
            guard.assist = false;
            guard.currentColor = UnityEngine.Color.Lerp(guard.currentColor, guard.guardVariables.chasingColor, UnityEngine.Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
