using UnityEngine;

public class StationaryIdle : Node
{
    public Guard guard;

    public StationaryIdle(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        float distance = Vector3.Distance(guard.transform.position, guard.guardMovement.path[0]);
        
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.1f && guard.guardMovement.idle)
        {
            guard.transform.rotation = Quaternion.Lerp(guard.transform.rotation, guard.guardMovement.targetRotation, 5.0f * Time.deltaTime);
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
