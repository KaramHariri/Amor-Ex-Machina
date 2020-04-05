public class Investigate : Node
{
    Guard guard;

    public Investigate(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.Investigate();
        float distance = UnityEngine.Vector3.Distance(guard.transform.position, guard.guardMovement.investigationPosition);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance)
        {
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}