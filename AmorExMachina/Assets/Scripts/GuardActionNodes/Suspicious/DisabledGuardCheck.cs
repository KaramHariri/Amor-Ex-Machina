public class DisabledGuardCheck : Node
{
    Guard guard;

    public DisabledGuardCheck(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (guard.sensing.FoundKnockedOutGuard())
        {
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
