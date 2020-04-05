using System.Collections.Generic;

public class SelectorNode : Node
{
    List<Node> childrenNodes = new List<Node>();

    public void AddChild(Node child)
    {
        childrenNodes.Add(child);
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        foreach (Node node in childrenNodes)
        {
            nodeState = node.Run();
            if (nodeState != NodeState.FAILURE)
            {
                break;
            }
        }
        return nodeState;
    }
}
