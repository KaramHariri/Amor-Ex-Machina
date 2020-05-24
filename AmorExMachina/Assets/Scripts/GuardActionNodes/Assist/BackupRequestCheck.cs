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
            //guard.guardMovement.animEnabled = true;
            guard.guardMovement.isWalking = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
