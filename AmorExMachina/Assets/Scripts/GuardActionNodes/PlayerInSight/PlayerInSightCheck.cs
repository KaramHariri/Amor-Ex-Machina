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
        if (guard.sensing.PlayerDetectedCheck())
        {
            guard.assist = false;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
