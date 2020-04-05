public class EnableGuard : Node
{
    Guard guard;

    public EnableGuard(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.sensing.disabledGuards[0].disabled = false;
        guard.sensing.disabledGuards.RemoveAt(0);
        nodeState = NodeState.SUCCESS;
        return nodeState;
    }
}
