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
            guard.guardMovement.isWalking = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
