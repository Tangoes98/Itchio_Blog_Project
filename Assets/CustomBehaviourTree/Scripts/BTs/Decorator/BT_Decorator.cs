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
        => AbortState;

    public override void NodeAbort()
    {
        //* Abort Child
        _child.NodeAbort();

        //* Abort Self
        base.NodeAbort();
        AbortState = NodeState.Failure;
    }
}