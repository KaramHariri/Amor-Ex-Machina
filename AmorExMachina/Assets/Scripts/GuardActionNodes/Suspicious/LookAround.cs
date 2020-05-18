using UnityEngine;

public class LookAround : Node
{
    Guard guard;

    public LookAround(Guard agent)
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
            guard.guardMovement.idle = false;
            guard.assist = false;
            guard.sensing.suspicious = false;
            guard.sensing.distracted = false;
            guard.sensing.updatedRotation = false;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
