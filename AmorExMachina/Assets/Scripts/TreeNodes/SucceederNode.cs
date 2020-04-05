public class SucceederNode : Node
{
    Node childNode;
    public void AddChild(Node child)
    {
        childNode = child;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.SUCCESS;
        childNode.Run();
        return nodeState;
    }
}
