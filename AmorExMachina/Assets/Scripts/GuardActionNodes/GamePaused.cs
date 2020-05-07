public class GamePaused : Node
{
    public override NodeState Run()
    {
        NodeState nodeState = NodeState.FAILURE;
        if (GameHandler.currentState == GameState.MENU)
        {
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
