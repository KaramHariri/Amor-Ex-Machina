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
        guard.guardMovement.lookingAroundTimer -= Time.deltaTime;
        RotateGuardNeck();
        guard.guardMovement.isWalking = false;
        guard.guardMovement.animEnabled = false;
        if (guard.guardMovement.lookingAroundTimer <= 0)
        {
            guard.guardMovement.animEnabled = true;
            guard.guardMovement.isWalking = true;

            guard.guardMovement.idle = false;
            guard.assist = false;
            guard.sensing.suspicious = false;
            guard.sensing.distracted = false;
            guard.sensing.alarmed = false;
            guard.updatedRotation = false;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }

    void RotateGuardNeck()
    {
        Quaternion from = Quaternion.Euler(guard.lookingAroundPositiveVector);
        Quaternion to = Quaternion.Euler(guard.lookingAroundNegativeVector);

        float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * guard.lookingAroundFrequency));
        guard.guardNeckTransform.localRotation = Quaternion.Lerp(from, to, lerp);
    }
}
