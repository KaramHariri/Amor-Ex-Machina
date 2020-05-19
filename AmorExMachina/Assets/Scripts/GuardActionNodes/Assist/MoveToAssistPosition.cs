public class MoveToAssistPosition : Node
{
    Guard guard;

    public MoveToAssistPosition(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        float distance = UnityEngine.Vector3.Distance(guard.guardMovement.assistPosition, guard.transform.position);
        if (distance > guard.guardMovement.navMeshAgent.stoppingDistance + 0.1f)
        {
            guard.guardMovement.Assist();
            nodeState = NodeState.RUNNING;
        }
        else
        {
            guard.UpdateLookingAroundAngle();
            guard.guardMovement.idle = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
