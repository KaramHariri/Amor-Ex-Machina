using System.Collections.Generic;

public class SequenceNode : Node
{
    List<Node> childrenNodes = new List<Node>();

    public void AddChild(Node child)
    {
        childrenNodes.Add(child);
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.SUCCESS;
        foreach (Node node in childrenNodes)
        {
            nodeState = node.Run();
            if (nodeState != NodeState.SUCCESS)
            {
                break;
            }
        }
        return nodeState;
    }
}