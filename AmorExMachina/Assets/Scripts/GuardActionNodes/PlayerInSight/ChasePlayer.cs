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
        if(guard.sensing.detectionAmount >= guard.sensing.maxDetectionAmount)
            guard.guardMovement.ChasePlayer();
        float distance = UnityEngine.Vector3.Distance(guard.transform.position, guard.playerVariables.playerTransform.position);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance)
        {
            guard.guardMovement.navMeshAgent.isStopped = true;
            guard.playerVariables.caught = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
