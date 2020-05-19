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
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
