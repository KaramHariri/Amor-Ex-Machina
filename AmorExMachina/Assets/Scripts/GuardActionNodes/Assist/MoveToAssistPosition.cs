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
        UnityEngine.Vector3 assistPos = new UnityEngine.Vector3(guard.guardMovement.assistPosition.x, 1.0f, guard.guardMovement.assistPosition.z);
        float distance = UnityEngine.Vector3.Distance(assistPos, guard.transform.position);
        if (distance > guard.guardMovement.navMeshAgent.stoppingDistance)
        {
            guard.currentColor = UnityEngine.Color.Lerp(guard.currentColor, guard.guardVariables.chasingColor, UnityEngine.Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            guard.guardMovement.Assist();
            nodeState = NodeState.RUNNING;
        }
        else
        {
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
