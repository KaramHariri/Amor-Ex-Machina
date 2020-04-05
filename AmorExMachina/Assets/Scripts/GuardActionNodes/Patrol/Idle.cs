public class Idle : Node
{
    Guard guard;

    public Idle(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        float distance = UnityEngine.Vector3.Distance(guard.transform.position, guard.guardMovement.path[0]);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance && guard.guardMovement.idleTimer > 0 && guard.guardMovement.wayPointIndex == 0)
        {
            guard.currentColor = UnityEngine.Color.Lerp(guard.currentColor, guard.guardVariables.idleColor, UnityEngine.Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            guard.guardMovement.idleTimer -= UnityEngine.Time.deltaTime;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}