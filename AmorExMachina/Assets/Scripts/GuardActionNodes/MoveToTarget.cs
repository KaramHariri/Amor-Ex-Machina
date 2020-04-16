using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : Node
{
    Guard guard;

    public MoveToTarget(Guard agent)
    {
        guard = agent;
    }

    public override NodeState Run()
    {
        throw new System.NotImplementedException();
    }
}
