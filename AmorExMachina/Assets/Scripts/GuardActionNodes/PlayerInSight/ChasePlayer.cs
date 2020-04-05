public class ChasePlayer : Node
{
    Guard guard;

    public ChasePlayer(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.ChasePlayer();
        float distance = UnityEngine.Vector3.Distance(guard.transform.position, guard.playerVariables.playerTransform.position);
        if (distance <= 2.0f)
        {
            guard.playerVariables.caught = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
