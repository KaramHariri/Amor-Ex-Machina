public class LookAround : Node
{
    Guard guard;

    public LookAround(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.idleTimer -= UnityEngine.Time.deltaTime;
        if (guard.guardMovement.idleTimer <= 0)
        {
            guard.assist = false;
            guard.sensing.suspicious = false;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
