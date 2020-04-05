public class MoveToDisabledGuardPosition : Node
{
    Guard guard;

    public MoveToDisabledGuardPosition(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;

        float distance = UnityEngine.Vector3.Distance(guard.transform.position, guard.sensing.disabledGuards[0].transform.position);
        if (distance <= guard.sensing.navMeshAgent.stoppingDistance)
        {
            nodeState = NodeState.SUCCESS;
        }
        else
            guard.guardMovement.MoveTowardsKnockedOutGuard(guard.sensing.disabledGuards[0].transform.position);
        return nodeState;
    }
}
