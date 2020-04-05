public class Inverter : Node
{
    Node childNode;
    public void AddChild(Node child)
    {
        childNode = child;
    }

    public override NodeState Run()
    {
        NodeState nodeState = childNode.Run();
        if (nodeState == NodeState.SUCCESS)
            return NodeState.FAILURE;
        else if (nodeState == NodeState.FAILURE)
            return NodeState.SUCCESS;
        return nodeState;
    }
}
