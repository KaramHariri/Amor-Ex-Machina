public class TalkToOtherGuardCheck : Node
{
    Guard guard;

    public TalkToOtherGuardCheck(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        float distance = UnityEngine.Vector3.Distance(guard.transform.position, guard.guardMovement.path[0]);
        if (distance <= guard.guardMovement.navMeshAgent.stoppingDistance && guard.guardMovement.talkingTimer > 0 && guard.guardMovement.CloseGuardNearby())
        {
            guard.currentColor = UnityEngine.Color.Lerp(guard.currentColor, guard.guardVariables.talkingToOtherGuardColor, UnityEngine.Time.deltaTime);
            guard.meshRenderer.material.color = guard.currentColor;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}