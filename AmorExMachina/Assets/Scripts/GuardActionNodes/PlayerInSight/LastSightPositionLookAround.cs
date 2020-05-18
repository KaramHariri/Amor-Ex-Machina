using UnityEngine;

public class LastSightPositionLookAround : Node
{
    Guard guard;

    public LastSightPositionLookAround(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.guardMovement.idleTimer -= Time.deltaTime;
        Quaternion from = Quaternion.Euler(guard.positiveVector);
        Quaternion to = Quaternion.Euler(guard.negativeVector);

        float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * guard.m_frequency));
        guard.transform.localRotation = Quaternion.Lerp(from, to, lerp);
        if (guard.guardMovement.idleTimer <= 0)
        {
            guard.sensing.playerWasInSight = false;
            guard.sensing.updatedRotation = false;
            guard.guardMovement.idleTimer = 5.0f;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
