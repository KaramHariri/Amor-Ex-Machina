public class EnableGuard : Node
{
    Guard guard;

    public EnableGuard(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        NodeState nodeState = NodeState.RUNNING;
        guard.sensing.disabledGuardsFound[0].disabled = false;
        guard.sensing.disabledGuardsFound[0].PlayerEnableVFX();
        //Added 2020-05-21
        guard.sensing.disabledGuardsFound[0].guardMovement.isDisabled = false;
        guard.PlayEnablingSound(guard.sensing.disabledGuardsFound[0].transform.position);
        guard.sensing.disabledGuardsFound.RemoveAt(0);
        nodeState = NodeState.SUCCESS;
        return nodeState;
    }
}
