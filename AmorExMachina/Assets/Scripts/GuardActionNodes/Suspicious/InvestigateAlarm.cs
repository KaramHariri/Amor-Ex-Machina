using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateAlarm : Node
{
    Guard guard;

    public InvestigateAlarm(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.AlarmInvestigate();
        float distance = Vector3.Distance(guard.transform.position, guard.guardMovement.alarmInvestigationPosition);
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
