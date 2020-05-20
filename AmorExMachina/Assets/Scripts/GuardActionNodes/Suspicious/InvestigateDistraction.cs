using UnityEngine;

public class InvestigateDistraction : Node
{
    Guard guard;

    public InvestigateDistraction(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.DistractionInvestigate();
        float distance = Vector3.Distance(guard.transform.position, guard.guardMovement.distractionInvestigationPosition);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.2f)
        {
            guard.UpdateLookingAroundAngle();
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
