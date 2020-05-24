﻿using UnityEngine;

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
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance + 0.3f)
        {
            guard.guardMovement.idle = true;
            guard.guardMovement.reachedDestination = true;
            //guard.UpdateLookingAroundAngle();
            nodeState = NodeState.SUCCESS;
        }
        else
        {
            guard.guardMovement.reachedDestination = false;
        }
        return nodeState;
    }
}
