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
        guard.guardMovement.ResetIdleTimer();
        guard.guardMovement.FollowPath();
        return nodeState;
    }
}