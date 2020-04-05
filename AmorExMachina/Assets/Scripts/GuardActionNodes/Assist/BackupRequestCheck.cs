public class BackupRequestCheck : Node
{
    Guard guard;

    public BackupRequestCheck(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (guard.assist)
        {
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
