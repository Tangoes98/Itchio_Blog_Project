using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Task : BT_Node
{
    public override void NodeAbort()
    {
        base.NodeAbort();
        AbortState = NodeState.Failure;
    }

    protected override void NodeExit()
        => AbortState = NodeState.Running;

}
