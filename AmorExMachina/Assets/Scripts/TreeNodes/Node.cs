public enum NodeState
{
    SUCCESS,
    FAILURE,
    RUNNING
}

public abstract class Node
{
    public abstract NodeState Run();
}
