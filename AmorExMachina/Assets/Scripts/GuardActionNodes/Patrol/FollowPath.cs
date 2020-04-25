public class FollowPath : Node
{
    Guard guard;

    public FollowPath(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        nodeState = NodeState.SUCCESS;
        guard.currentColor = UnityEngine.Color.Lerp(guard.currentColor, guard.guardVariables.patrolColor, UnityEngine.Time.deltaTime);
        guard.meshRenderer.material.color = guard.currentColor;

        guard.guardMovement.ResetIdleTimer();
        guard.guardMovement.FollowPath();
        return nodeState;
    }
}