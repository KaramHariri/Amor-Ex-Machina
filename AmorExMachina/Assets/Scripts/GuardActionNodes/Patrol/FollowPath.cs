using UnityEngine;

public class FollowPath : Node
{
    Guard guard;

    public FollowPath(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        guard.guardMovement.ResetIdleTimer();
        guard.guardMovement.FollowPath();
        guard.guardMovement.isChasingPlayer = false;
        RotateGuardNeck();
        return nodeState;
    }

    void RotateGuardNeck()
    {
        float guardAngleY = guard.transform.eulerAngles.y;
        if(guardAngleY > 180.0f)
        {
            guardAngleY -= 360.0f;
        }
        else if(guardAngleY < -180.0f)
        {
            guardAngleY += 360.0f;
        }

        float neckAngleY = guard.guardNeckTransform.localEulerAngles.y;
        if(neckAngleY > 180.0f)
        {
            neckAngleY -= 360.0f;
        }
        else if(neckAngleY < -180.0f)
        {
            neckAngleY += 360.0f;
        }

        Quaternion forwardDirection = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Quaternion currentNeckRotation = Quaternion.Euler(0.0f, neckAngleY, 0.0f);

        guard.guardNeckTransform.localRotation = Quaternion.Lerp(currentNeckRotation, forwardDirection, Time.realtimeSinceStartup);
    }
}