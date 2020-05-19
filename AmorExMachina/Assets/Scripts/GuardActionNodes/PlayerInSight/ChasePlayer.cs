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
        float distance = UnityEngine.Vector3.Distance(guard.transform.position, guard.playerTransform.position);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance)
        {
            guard.guardMovement.navMeshAgent.isStopped = true;
            GameHandler.playerIsCaught = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
