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
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.3f)
        {
            guard.guardMovement.navMeshAgent.isStopped = true;
            guard.guardMovement.reachedDestination = true;
            GameHandler.playerIsCaught = true;
            nodeState = NodeState.SUCCESS;
        }
        else
        {
            guard.guardMovement.reachedDestination = false;
        }
        return nodeState;
    }
}
