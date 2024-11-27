using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Decorator : BT_Node
{
    protected BT_Node _child;
    public BT_Decorator(BT_Node child)
    {
        _child = child;
    }

    protected override NodeState NodeEnter()
    {
        return _abortState;
    }

    public override void NodeAbort()
    {
        _child.NodeAbort();
        //base.NodeAbort();
        _abortState = NodeState.Failure;
    }

}
