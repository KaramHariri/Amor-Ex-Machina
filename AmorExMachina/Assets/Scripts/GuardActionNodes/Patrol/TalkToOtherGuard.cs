public class TalkToOtherGuard : Node
{
    Guard guard;

    public TalkToOtherGuard(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.SUCCESS;
        guard.guardMovement.talkingTimer -= UnityEngine.Time.deltaTime;
        return nodeState;
    }
}