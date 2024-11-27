using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Selector : BT_CompositeNode
{
    protected override NodeState NodeTick()
    {
        NodeState result = CurrentChildNode().RunNode();

        if (result == NodeState.Success)
            return NodeState.Success;

        if (result == NodeState.Failure && !GetNextChildNode())
            return NodeState.Failure;

        return NodeState.Running;
    }

    public override void NodeAbort()
    {
        CurrentChildNode().NodeAbort();
        base.NodeAbort();
        _abortState = NodeState.Success;
    }
}
