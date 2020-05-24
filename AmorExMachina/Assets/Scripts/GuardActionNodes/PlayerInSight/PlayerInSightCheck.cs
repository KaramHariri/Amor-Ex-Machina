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
            //guard.guardMovement.animEnabled = true;
            guard.guardMovement.isWalking = true;
            guard.guardMovement.isChasingPlayer = true;
            guard.assist = false;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
