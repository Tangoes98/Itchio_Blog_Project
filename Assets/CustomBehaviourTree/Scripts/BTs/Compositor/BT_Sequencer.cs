using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Sequencer : BT_CompositeNode
{
    protected override NodeState NodeTick()
    {
        NodeState result = CurrentChildNode().RunNode();

        if (result == NodeState.Failure)
            return NodeState.Failure;

        if (result == NodeState.Success && !GetNextChildNode())
            return NodeState.Success;

        return NodeState.Running;
    }

    public override void NodeAbort()
    {
        //*Abort Child
        CurrentChildNode().NodeAbort();
        
        //*Abort Self
        base.NodeAbort();
        AbortState = NodeState.Failure;

    }
}
